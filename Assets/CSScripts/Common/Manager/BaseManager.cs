using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoXu.Core
{
    /// <summary>
    /// 系统状态
    /// </summary>
    public enum ManagerStatus
    {
        Nil, // 未初始化
        Awaked, // 已经唤醒过
        Preparing, // 初始化过程中
        Running, // 执行过程中
        Disposed, // 已销毁
    }
    /// <summary>
    /// 管理类基类
    /// </summary>
    public abstract class BaseManager: MonoBehaviour
    {
        public abstract void OnInit();

        public abstract void OnUpdate();

        public abstract void OnFixedUpdate();

        public abstract void OnLateUpdate();

        public abstract void OnDispose();
    }
}
