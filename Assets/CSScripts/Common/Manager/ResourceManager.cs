using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace XiaoXu.Core
{
	public class ResourceManager : BaseManager
	{
		private ResLoadManager resLoadManager;
		private PoolManager poolManager;

		// 应该使用对象池的类型
		private HashSet<string> poolableAssets = new HashSet<string>
		{
			"Enemy_", 
		};
		public override void OnInit()
		{
			resLoadManager = GameMain.resLoadManager;
			poolManager = GameMain.poolManager;
		}
		public override void OnUpdate() { }
		public override void OnFixedUpdate() { }
		public override void OnLateUpdate() { }
		public override void OnDispose() { }

        // 对外接口：加载单个资源
        public async Task<T> LoadAssetAsync<T>(string assetKey) where T : UnityEngine.Object
		{
			return await resLoadManager.LoadAssetAsync<T>(assetKey);
		}
        // 对外接口：释放单个资源
        public void ReleaseAsset(string assetKey)
		{
			resLoadManager.ReleaseAsset(assetKey);
		}
		// 对外接口：实例化
		public async Task<GameObject> InstantiateAsync(string assetKey, Vector3 position, Quaternion rotation)
		{
			if (ShouldUsePool(assetKey))
			{
				Debug.Log($"{assetKey}使用对象池实例化");
				return await InstantiateWithPool(assetKey, position, rotation);
			}
			else
			{
                Debug.Log($"{assetKey}直接实例化不使用对象池");
                return await InstantiateDirect(assetKey, position, rotation);
			}
		}
		// 用对象池实例化
		public async Task<GameObject> InstantiateWithPool(string assetKey, Vector3 position, Quaternion rotation)
		{
			if (!poolManager.HasPool(assetKey))
			{
				await CreatePoolForAsset(assetKey);
			}
			return poolManager.GetFromPool(assetKey, position, rotation);
		}
		// 不用对象池，调用Resload的实例化接口
		public async Task<GameObject> InstantiateDirect(string assetKey, Vector3 position, Quaternion rotation)
		{
			return await resLoadManager.DirectInstantiateAsync(assetKey, position, rotation);
		}
		// 对外接口：摧毁实例化
		public void DestroyInstance(GameObject instance)
		{
			if (instance == null) return;

			PoolObject poolObject = instance.GetComponent<PoolObject>();
			if (poolObject != null && !string.IsNullOrEmpty(poolObject.poolName))
			{
				poolManager.RecycleToPool(poolObject.poolName, instance);
			}
			else
			{
                DestroyDirect(instance);
			}
		}
		// 用对象池回收
		public void RecycleToPool(GameObject instance)
		{
			PoolObject poolObject = instance.GetComponent<PoolObject>();
			if (poolObject != null && !string.IsNullOrEmpty(poolObject.poolName))
			{
				poolManager.RecycleToPool(poolObject.poolName, instance);
			}
			else
			{
				Debug.LogWarning("对象没有PoolObject组件，无法回收到对象池，直接释放");
                DestroyDirect(instance);
			}
		}
        // 不用对象池，调用Resload直接摧毁实例化
        public void DestroyDirect(GameObject instance)
		{
			resLoadManager.DirectReleaseInstance(instance);
		}
		// 为资产创建池
		public async Task CreatePoolForAsset(string assetKey, int poolSize = 10, int maxSize = 30)
		{
			if (poolManager.HasPool(assetKey)) return;

			GameObject prefab = await resLoadManager.LoadAssetAsync<GameObject>(assetKey);
			if (prefab != null)
			{
				poolManager.CreatePool(assetKey, prefab, poolSize, maxSize);
				Debug.Log($"使用对象池实例化，创建出池子:{assetKey}");
			}
		}
		public bool HasPool(string assetKey) => poolManager.HasPool(assetKey);
		public void ClearPool(string assetKey) => poolManager.ClearPool(assetKey);
		// 标签加载
		public async Task<List<T>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
		{
			return await resLoadManager.LoadAssetsByLabelAsync<T>(label);
		}
		// 标签释放
		public void ReleaseAssetsByLabel(string label)
		{
			resLoadManager.ReleaseAssetsByLabel(label);
		}
		// 判断是否适合使用对象池
		private bool ShouldUsePool(string assetKey)
		{
			foreach (string pattern in poolableAssets)
			{
				if (assetKey.Contains(pattern)) //如果名字里包含就用
				{
					return true;
				}
			}
			return false;
		}
        // 调用ResLoadManager看资源是否被加载
        public bool IsAssetLoaded(string assetKey) => resLoadManager.IsAssetLoaded(assetKey);
        // 调用ResLoadManager看资源的引用数量
        public int GetAssetRefCount(string assetKey) => resLoadManager.GetAssetRefCount(assetKey);
        // 调用poolManager看池中现有物体和所有物体数量
        public (int available, int total) GetPoolStatus(string poolName) => poolManager.GetPoolStatus(poolName);
	}
}