using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using System.Linq;

namespace XiaoXu.Core
{
	public class ResLoadManager : BaseManager
	{
		private class AssetInfo
		{
			public AsyncOperationHandle Handle;
			public int RefCount = 0;
		}

		private Dictionary<string, AssetInfo> assetInfos = new Dictionary<string, AssetInfo>();

		public override void OnInit() { }
		public override void OnUpdate() { }
		public override void OnFixedUpdate() { }
		public override void OnLateUpdate() { }

		public override void OnDispose()
		{
			foreach (var assetInfo in assetInfos.Values)
			{
				if (assetInfo.Handle.IsValid())
				{
					Addressables.Release(assetInfo.Handle);
				}
			}
			assetInfos.Clear();
		}

		// 加载单个资源
		public async Task<T> LoadAssetAsync<T>(string assetKey) where T : UnityEngine.Object
		{
			try
			{
				if (assetInfos.TryGetValue(assetKey, out var assetInfo))
				{
					assetInfo.RefCount++;
					return (T)assetInfo.Handle.Result;
				}

				AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetKey);
				await handle.Task;

				assetInfo = new AssetInfo
				{
					Handle = handle,
					RefCount = 1
				};

				assetInfos[assetKey] = assetInfo;
				return handle.Result;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"加载失败 {assetKey}");
				return null;
			}
		}

		// 释放单个资源
		public void ReleaseAsset(string assetKey)
		{
			if (assetInfos.TryGetValue(assetKey, out var assetInfo))
			{
				assetInfo.RefCount--;
				if (assetInfo.RefCount <= 0)
				{
					if (assetInfo.Handle.IsValid())
					{
						Addressables.Release(assetInfo.Handle);
					}
					assetInfos.Remove(assetKey);
				}
			}
		}

		// 直接实例化，不用对象池
		public async Task<GameObject> DirectInstantiateAsync(string assetKey, Vector3 position, Quaternion rotation)
		{
			try
			{
				AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetKey, position, rotation);
				GameObject instance = await handle.Task;
				return instance;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"实例化失败 {assetKey}");
				return null;
			}
		}

		// 直接释放实例
		public void DirectReleaseInstance(GameObject instance)
		{
			if (instance != null)
			{
				Addressables.ReleaseInstance(instance);
			}
		}

		// 通过Label加载多个资源
		public async Task<List<T>> LoadAssetsByLabelAsync<T>(string label) where T : UnityEngine.Object
		{
			try
			{
				var handle = Addressables.LoadAssetsAsync<T>(label, null);
				var results = await handle.Task;
				List<T> resultList = results.ToList();

				foreach (var result in resultList)
				{
					string key = result.name;
					if (!assetInfos.ContainsKey(key))
					{
						assetInfos[key] = new AssetInfo
						{
							Handle = handle,
							RefCount = 1
						};
					}
					else
					{
						assetInfos[key].RefCount++;
					}
				}

				return resultList;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"通过Label加载失败 {label}");
				return new List<T>();
			}
		}

		// 释放用Label加载的资源
		public void ReleaseAssetsByLabel(string label)
		{
			List<string> toRemove = new List<string>();
			foreach (var kvp in assetInfos)
			{
				if (kvp.Key.Contains(label))
				{
					toRemove.Add(kvp.Key);
				}
			}
			foreach (string key in toRemove)
			{
				ReleaseAsset(key);
			}
		}

		// 获取资源信息
		public bool IsAssetLoaded(string assetKey) => assetInfos.ContainsKey(assetKey);
		public int GetAssetRefCount(string assetKey) => assetInfos.TryGetValue(assetKey, out var info) ? info.RefCount : 0;
	}
}