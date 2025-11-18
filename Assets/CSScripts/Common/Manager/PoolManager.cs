using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoXu.Core
{
	public class PoolManager : BaseManager
	{
		[System.Serializable]

		//对象池
		private class Pool
		{
			public string poolName;
			public GameObject prefab;
			public Queue<GameObject> availableObjects = new Queue<GameObject>();
			public List<GameObject> allObjects = new List<GameObject>();
			public int maxSize;
		}
		//池字典，按名查找
		private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
		private Transform poolRoot;

		public override void OnInit()
		{
			GameObject rootObj = new GameObject("PoolRoot");
			poolRoot = rootObj.transform;
			poolRoot.SetParent(transform);
		}

		public override void OnUpdate() { }
		public override void OnFixedUpdate() { }
		public override void OnLateUpdate() { }
		public override void OnDispose()
		{
			ClearAllPools();
		}

		// 创建对象池
		public void CreatePool(string poolName, GameObject prefab, int initSize = 10, int maxSize = 50)
		{
			if (pools.ContainsKey(poolName))
			{
				Debug.LogWarning($"对象池 {poolName} 已存在");
				return;
			}

			Pool pool = new Pool
			{
				poolName = poolName,
				prefab = prefab,
				maxSize = maxSize
			};

			//预加载池中物体
			for (int i = 0; i < initSize; i++)
			{
				GameObject obj = CreateNewObject(prefab, poolName);
				obj.SetActive(false);
				pool.availableObjects.Enqueue(obj);
				pool.allObjects.Add(obj);
			}

			pools[poolName] = pool;
		}
		// 从对象池获取对象
		public GameObject GetFromPool(string poolName, Vector3 position, Quaternion rotation)
		{
			if (!pools.ContainsKey(poolName))
			{
				Debug.LogError($"对象池 {poolName} 不存在");
				return null;
			}

			Pool pool = pools[poolName];
			GameObject obj = null;

			if (pool.availableObjects.Count > 0)
			{
				obj = pool.availableObjects.Dequeue();
			}
			else if (pool.allObjects.Count < pool.maxSize) //没有那就实例化出一个
			{
				obj = CreateNewObject(pool.prefab, poolName);
				pool.allObjects.Add(obj);
			}
			else
			{
				Debug.LogWarning($"对象池 {poolName} 已满");
				return null;
			}

			if (obj != null)
			{
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				obj.SetActive(true);

				PoolObject poolObject = obj.GetComponent<PoolObject>();
				if (poolObject != null)
					poolObject.OnSpawn();
			}

			return obj;
		}
		// 回收到对象池
		public void RecycleToPool(string poolName, GameObject obj)
		{
			if (!pools.ContainsKey(poolName) || obj == null) return;

			Pool pool = pools[poolName];

			PoolObject poolObject = obj.GetComponent<PoolObject>();
			if (poolObject != null)
				poolObject.OnDespawn();

			obj.SetActive(false);
			obj.transform.SetParent(poolRoot);

			if (!pool.availableObjects.Contains(obj))
			{
				pool.availableObjects.Enqueue(obj);
			}
		}
        // 有没有该池
        public bool HasPool(string poolName) => pools.ContainsKey(poolName);
        // 按名删除池，摧毁池中物体
        public void ClearPool(string poolName)
		{
			if (pools.ContainsKey(poolName))
			{
				Pool pool = pools[poolName];
				foreach (var obj in pool.allObjects)
				{
					if (obj != null) Destroy(obj);
				}
				pools.Remove(poolName);
			}
		}
		// 删除所有池
		public void ClearAllPools()
		{
			foreach (var pool in pools.Values)
			{
				foreach (var obj in pool.allObjects)
				{
					if (obj != null) Destroy(obj);
				}
			}
			pools.Clear();
		}
		// 查询池中现有物体和所有物体数量
		public (int available, int total) GetPoolStatus(string poolName)
		{
			if (pools.ContainsKey(poolName))
			{
				var pool = pools[poolName];
				return (pool.availableObjects.Count, pool.allObjects.Count);
			}
			return (0, 0);
		}
		//实例化池中物体
		private GameObject CreateNewObject(GameObject prefab, string poolName)
		{
			GameObject obj = Instantiate(prefab); //TODO：思考要不要在这引用ResLoad的方法
			obj.transform.SetParent(poolRoot);

			PoolObject poolObject = obj.GetComponent<PoolObject>();
			if (poolObject == null) 
				poolObject = obj.AddComponent<PoolObject>();
			poolObject.poolName = poolName;

			return obj;
		}
	}
}