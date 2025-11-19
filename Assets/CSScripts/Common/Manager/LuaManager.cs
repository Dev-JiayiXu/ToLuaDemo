using System;
using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;

namespace XiaoXu.Core
{
    public class LuaManager : BaseManager
    {
        public LuaState luaState; // 获取虚拟机
		public override void OnInit()
		{
			luaState = new LuaState();
			luaState.Start();
			LuaBinder.Bind(luaState);
			luaState.AddSearchPath(LuaConst.luaDir);

			// 延迟一帧执行Lua框架初始化，不然LuaManager就无法在Init()中获取到ResourceManager
			StartCoroutine(DelayedLuaInit());
		}
		private IEnumerator DelayedLuaInit()
		{
			// 等一帧再进入LuaEntry
			yield return null;
			luaState.Require("LuaEntry");
		}

		public override void OnUpdate()
        {
        }
        public override void OnFixedUpdate()
        {
        }
        public override void OnLateUpdate()
        {
        }
        public override void OnDispose()
        {
            luaState.Dispose();
        }
    }
}
