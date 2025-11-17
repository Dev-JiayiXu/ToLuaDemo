using System;
using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;

namespace XiaoXu.Core
{
    public class LuaManager : BaseManager
    {
        protected LuaState luaState; // 获取虚拟机
        public override void OnInit() //初始化虚拟机、读入lua文件
        {
            luaState = new LuaState();
            luaState.Start();
            LuaBinder.Bind(luaState);
            luaState.AddSearchPath(LuaConst.luaDir);
            luaState.Require("Main");
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
