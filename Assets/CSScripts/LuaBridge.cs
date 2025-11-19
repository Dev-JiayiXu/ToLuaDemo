// LuaBridge.cs - 通用桥接组件
using UnityEngine;
using XiaoXu.Core;

public class LuaBridge : MonoBehaviour
{
	[Header("Lua配置")]
	public string luaClassName = "LoadTest"; // 要调用的Lua类名

	[Header("LoadTest参数")]
	public string label;
	public UnityEngine.UI.Image targetImage;
	public string imageSpriteAddress;
	public string enemyName;
	public Transform spawnParent;
	public Vector3 spawnPosition = Vector3.zero;

	private void Start()
	{
		ExecuteLuaStart();
	}

	private void OnDestroy()
	{
		ExecuteLuaDestroy();
	}

	public void ExecuteLuaStart()
	{
		LuaManager luaManager = GameMain.luaManager;
		if (luaManager != null && luaManager.luaState != null)
		{
			// 在Lua中创建实例并设置参数
			luaManager.luaState.DoString($@"
                -- 创建Lua实例
                _G.currentLuaInstance = New({luaClassName})
                
                -- 设置参数
                _G.currentLuaInstance.label = '{label}'
                _G.currentLuaInstance.targetImage = targetImage
                _G.currentLuaInstance.imageSpriteAddress = '{imageSpriteAddress}'
                _G.currentLuaInstance.enemyName = '{enemyName}'
                _G.currentLuaInstance.spawnParent = spawnParent
                _G.currentLuaInstance.spawnPosition = UnityEngine.Vector3({spawnPosition.x}, {spawnPosition.y}, {spawnPosition.z})
                _G.currentLuaInstance.spawnRotation = UnityEngine.Quaternion.identity
                
                -- 调用Start方法
                _G.currentLuaInstance:Start()
            ");
		}
	}

	public void ExecuteLuaDestroy()
	{
		LuaManager luaManager = GameMain.luaManager;
		if (luaManager != null && luaManager.luaState != null)
		{
			luaManager.luaState.DoString(@"
                if _G.currentLuaInstance and _G.currentLuaInstance.OnDestroy then
                    _G.currentLuaInstance:OnDestroy()
                    _G.currentLuaInstance = nil
                end
            ");
		}
	}

	// 其他脚本可以通过这个方法来调用Lua方法
	public void CallLuaMethod(string methodName, params object[] args)
	{
		LuaManager luaManager = GameMain.luaManager;
		if (luaManager != null && luaManager.luaState != null)
		{
			string argString = "";
			if (args != null && args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i] is string)
						argString += $"'{args[i]}'";
					else
						argString += args[i].ToString();

					if (i < args.Length - 1)
						argString += ", ";
				}
			}

			luaManager.luaState.DoString($@"
                if _G.currentLuaInstance and _G.currentLuaInstance.{methodName} then
                    _G.currentLuaInstance:{methodName}({argString})
                end
            ");
		}
	}
}