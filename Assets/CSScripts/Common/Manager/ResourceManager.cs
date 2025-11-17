using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace XiaoXu.Core
{
	public class ResourceManager : BaseManager
	{
		private ResLoadManager resLoadManager;
		private PoolManager poolManager;

		// 应该使用对象池的资源类型
		private HashSet<string> poolableAssets = new HashSet<string>
		{
			"UI_", "Bullet_", "Effect_"
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

		// 资源加载接口
		public async Task<T> LoadAssetAsync<T>(string assetKey) where T : UnityEngine.Object
		{
			return await resLoadManager.LoadAssetAsync<T>(assetKey);
		}

		public void ReleaseAsset(string assetKey)
		{
			resLoadManager.ReleaseAsset(assetKey);
		}

		// 判断是否使用对象池
		public async Task<GameObject> InstantiateAsync(string assetKey, Vector3 position, Quaternion rotation)
		{
			if (ShouldUsePool(assetKey))
			{
				return await InstantiateWithPool(assetKey, position, rotation);
			}
			else
			{
				return await resLoadManager.DirectInstantiateAsync(assetKey, position, rotation);
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

		// 直接实例化，不使用对象池
		public async Task<GameObject> InstantiateDirect(string assetKey, Vector3 position, Quaternion rotation)
		{
			return await resLoadManager.DirectInstantiateAsync(assetKey, position, rotation);
		}

		// 判断回收方式
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
				resLoadManager.DirectReleaseInstance(instance);
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
				Debug.LogWarning("对象没有PoolObject组件，无法回收到对象池");
				resLoadManager.DirectReleaseInstance(instance);
			}
		}

		// 直接释放
		public void DestroyDirect(GameObject instance)
		{
			resLoadManager.DirectReleaseInstance(instance);
		}

		// 对象池管理
		public async Task CreatePoolForAsset(string assetKey, int poolSize = 10, int maxSize = 30)
		{
			if (poolManager.HasPool(assetKey)) return;

			GameObject prefab = await resLoadManager.LoadAssetAsync<GameObject>(assetKey);
			if (prefab != null)
			{
				poolManager.CreatePool(assetKey, prefab, poolSize, maxSize);
			}
		}

		public bool HasPool(string assetKey) => poolManager.HasPool(assetKey);
		public void ClearPool(string assetKey) => poolManager.ClearPool(assetKey);

		// 标签加载
		public async Task<List<T>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
		{
			return await resLoadManager.LoadAssetsByLabelAsync<T>(label);
		}

		public void ReleaseAssetsByLabel(string label)
		{
			resLoadManager.ReleaseAssetsByLabel(label);
		}

		// 判断是否适合使用对象池
		private bool ShouldUsePool(string assetKey)
		{
			foreach (string pattern in poolableAssets)
			{
				if (assetKey.Contains(pattern))
				{
					return true;
				}
			}
			return false;
		}

		// 添加/移除对象池资源类型
		public void AddPoolableAssetPattern(string pattern) => poolableAssets.Add(pattern);
		public void RemovePoolableAssetPattern(string pattern) => poolableAssets.Remove(pattern);

		// 状态查询
		public bool IsAssetLoaded(string assetKey) => resLoadManager.IsAssetLoaded(assetKey);
		public int GetAssetRefCount(string assetKey) => resLoadManager.GetAssetRefCount(assetKey);
		public (int available, int total) GetPoolStatus(string poolName) => poolManager.GetPoolStatus(poolName);
	}
}