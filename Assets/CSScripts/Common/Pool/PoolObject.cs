using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoXu.Core
{ 
    //对象池中物体参数管理
    public class PoolObject : MonoBehaviour
    {
        [HideInInspector]
        public string poolName;

        public virtual void OnSpawn()
        {
            // 对象被取出时的初始化
        }

        public virtual void OnDespawn()
        {
            // 对象被回收时的清理
        }
    }
}