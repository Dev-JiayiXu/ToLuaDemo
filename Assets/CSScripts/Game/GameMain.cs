using System;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoXu.Core
{
    /// <summary>
    /// 主脚本单例类，负责驱动所有Manager
    /// </summary>
    public class GameMain : MonoBehaviour
    {
        #region 单例实例
        private static GameMain _instance;
        public static GameMain Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameMain>();

                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameMain");
                        _instance = go.AddComponent<GameMain>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        private List<BaseManager> managerList = new List<BaseManager>();

        #region 生命周期函数        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Init();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            Remove();
        }
        #endregion

        void Init() //初始化所有管理器
        {
            AddAllManagers();
            foreach (var manager in managerList)
            {
                manager.OnInit();
            }
        }

        void Remove() //释放所有管理器资源
        {
            for (int i = managerList.Count - 1; i >= 0; i--) //逆序避开顺序依赖
            {
                if (managerList[i] != null)
                {
                    managerList[i].OnDispose();
                }
            }
            managerList.Clear();
        }
        public static LuaManager luaManager { get; protected set; }
        public static ResLoadManager resLoadManager { get; protected set; }
        public static PoolManager poolManager { get; protected set; }
		public static ResourceManager resourceManager { get; protected set; }
		void AddAllManagers() //注册管理器
        {
            luaManager = AddManager<LuaManager>("LuaManager");
            resLoadManager = AddManager<ResLoadManager>("ResLoadManager");
            poolManager = AddManager<PoolManager>("PoolManager");
			resourceManager = AddManager<ResourceManager>("ResourceManager");
		}

        public T AddManager<T>(string name) where T : BaseManager
        {
            var sys = this.GetComponent<T>();
            if(sys == null)
            {
                var obj = new GameObject(name);
                obj.transform.SetParent(this.transform);
                sys = obj.AddComponent<T>();
            }
            managerList.Add(sys);
            return sys;
        }
    }
}