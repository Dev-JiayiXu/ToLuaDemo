using LuaInterface;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XiaoXu.Core
{
	/// <summary>
	/// 绑定上来的UI组件
	/// </summary>
	[System.Serializable]
	public class UIComponent
	{
		public string componentName; // 组件在Lua中的名字
		public Component component; // Unity组件
	}
	/// <summary>
	/// Lua侧控制UI的桥接器
	/// </summary>
	public class UILuaBridge : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[Header("对应的ViewLua脚本")]
		public string ViewLuaName;
		[Header("对应的ProxyLua脚本")]
		public string ProxyLuaName;

		[Header("UI组件")]
		public List<UIComponent> components = new List<UIComponent>();

		private LuaTable luaView;
		private LuaTable luaProxy;
		private bool isInitialized = false;

		private void Start()
		{
			InitializeLuaController();
		}

		private void OnDestroy()
		{
			DestroyLuaController();
		}

		private void Update()
		{
			if (isInitialized && luaView != null)
			{
				// 调用Lua的Update方法
				LuaFunction updateFunc = luaView.GetLuaFunction("OnUpdate");
				if (updateFunc != null)
				{
					updateFunc.BeginPCall();
					updateFunc.PCall();
					updateFunc.EndPCall();
					updateFunc.Dispose();
				}
			}
		}

		/// <summary>
		/// 初始化Lua控制器
		/// </summary>
		public void InitializeLuaController()
		{
			if (string.IsNullOrEmpty(ViewLuaName) || isInitialized)
				return;

			try
			{
				LuaManager luaManager = GameMain.luaManager;
				if (luaManager == null || luaManager.luaState == null)
				{
					Debug.LogError("LuaManager未初始化");
					return;
				}

				// 创建Lua视图实例
				luaManager.luaState.DoString($@"
                local viewClass = require('{ViewLuaName}')
                _G.currentView = viewClass.New()
            ");

				luaView = luaManager.luaState.GetTable("currentView");
				if (luaView == null)
				{
					Debug.LogError($"创建Lua视图失败: {ViewLuaName}");
					return;
				}

				// 创建Lua代理实例（如果存在）
				if (!string.IsNullOrEmpty(ProxyLuaName))
				{
					luaManager.luaState.DoString($@"
                    local proxyClass = require('{ProxyLuaName}')
                    _G.currentProxy = proxyClass.New()
                ");

					luaProxy = luaManager.luaState.GetTable("currentProxy");

					// 将代理设置给视图
					if (luaProxy != null)
					{
						LuaFunction setProxyFunc = luaView.GetLuaFunction("SetProxy");
						if (setProxyFunc != null)
						{
							setProxyFunc.BeginPCall();
							setProxyFunc.Push(luaProxy);
							setProxyFunc.PCall();
							setProxyFunc.EndPCall();
							setProxyFunc.Dispose();
						}
					}
				}

				// 设置视图数据
				luaView["gameObject"] = gameObject;
				luaView["transform"] = transform;

				// 传递组件信息到Lua
				SetupComponents();

				// 调用Lua的初始化方法
				CallLuaMethod("OnStart");

				isInitialized = true;
				Debug.Log($"Lua视图初始化成功: {ViewLuaName}");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"初始化Lua视图失败: {ex.Message}");
			}
		}

		/// <summary>
		/// 设置组件
		/// </summary>
		private void SetupComponents()
		{
			if (luaView == null) return;

			// 创建组件表
			LuaTable componentsTable = GameMain.luaManager.luaState.NewTable();

			foreach (var component in components)
			{
				if (component.component != null && !string.IsNullOrEmpty(component.componentName))
				{
					componentsTable[component.componentName] = component.component;
				}
			}

			luaView["components"] = componentsTable;
		}

		/// <summary>
		/// 调用Lua方法
		/// </summary>
		public void CallLuaMethod(string methodName, params object[] args)
		{
			if (luaView == null) return;

			try
			{
				LuaFunction func = luaView.GetLuaFunction(methodName);
				if (func != null)
				{
					func.BeginPCall();
					foreach (var arg in args)
					{
						func.Push(arg);
					}
					func.PCall();
					func.EndPCall();
					func.Dispose();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"调用Lua方法失败 {methodName}: {ex.Message}");
			}
		}

		/// <summary>
		/// 销毁Lua控制器
		/// </summary>
		private void DestroyLuaController()
		{
			if (luaView != null)
			{
				CallLuaMethod("OnDestroy");
				luaView.Dispose();
				luaView = null;
			}
			if (luaProxy != null)
			{
				luaProxy.Dispose();
				luaProxy = null;
			}
			isInitialized = false;
		}

		// UI事件接口实现
		public void OnPointerClick(PointerEventData eventData)
		{
			CallLuaMethod("OnClick", eventData);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			CallLuaMethod("OnPointerEnter", eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			CallLuaMethod("OnPointerExit", eventData);
		}
	}
}