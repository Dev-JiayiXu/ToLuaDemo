using LuaInterface;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XiaoXu.Core;

/// <summary>
/// Lua控制的UI视图桥接器
/// </summary>
public class UILuaBridge : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Header("对应的ViewLua脚本")]
	public string ViewLuaName;

	[Header("UI组件")]
	public List<UIComponent> components = new List<UIComponent>();

	private LuaTable luaView;
	private bool isInitialized = false;

	private void Start()
	{
		InitializeLuaScript();
	}

	private void OnDestroy()
	{
		DestroyLuaScript();
	}

	private void Update()
	{
		if (isInitialized && luaView != null)
		{
			CallLuaMethod("OnUpdate");
		}
	}

	/// <summary>
	/// 初始化Lua控制器
	/// </summary>
	public void InitializeLuaScript()
	{
		if (string.IsNullOrEmpty(ViewLuaName) || isInitialized)
			return;

		LuaManager luaManager = GameMain.luaManager;
		if (luaManager == null || luaManager.luaState == null)
		{
			Debug.LogError("LuaManager未初始化");
			return;
		}

		// 直接执行Lua代码，不获取返回值
		luaManager.luaState.DoString($@"local viewClass = pcall(require, '{ViewLuaName}')");

		// 从全局变量获取创建的视图
		luaView = luaManager.luaState.GetTable("currentView");

		if (luaView == null)
		{
			Debug.LogError($"创建Lua视图失败: {ViewLuaName}");
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
				Debug.Log($"设置组件: {component.componentName} -> {component.component}");
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
					if (arg != null)
					{
						func.Push(arg);
					}
					else
					{
						func.Push((object)null);
					}
				}
				func.PCall();
				func.EndPCall();
				func.Dispose();
			}
			else
			{
				// 方法不存在是正常情况，不需要报错
				if (methodName != "OnUpdate")
				{
					Debug.Log($"Lua方法不存在: {methodName}");
				}
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
	private void DestroyLuaScript()
	{
		if (luaView != null)
		{
			CallLuaMethod("OnDestroy");
			luaView.Dispose();
			luaView = null;
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

	/// <summary>
	/// 检查Lua环境（调试用）
	/// </summary>
	[ContextMenu("检查Lua环境")]
	public void CheckLuaEnvironment()
	{
		LuaManager luaManager = GameMain.luaManager;
		if (luaManager == null || luaManager.luaState == null)
		{
			Debug.LogError("LuaManager未初始化");
			return;
		}

		luaManager.luaState.DoString(@"
            print('=== Lua环境检查 ===')
            print('package.path: ' .. package.path)
            
            -- 检查模块是否已加载
            if package.loaded['" + ViewLuaName + @"'] then
                print('模块 " + ViewLuaName + @" 已加载')
            else
                print('模块 " + ViewLuaName + @" 未加载')
            end
            
            -- 检查当前视图是否存在
            if _G.currentView then
                print('currentView 存在')
            else
                print('currentView 不存在')
            end
        ");
	}
}

/// <summary>
/// UI组件绑定
/// </summary>
[System.Serializable]
public class UIComponent
{
	public string componentName; // 组件在Lua中的字段名
	public Component component; // Unity组件
}