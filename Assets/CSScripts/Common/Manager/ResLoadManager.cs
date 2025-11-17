using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

namespace XiaoXu.Core
{
    public class ResLoadManager : BaseManager
    {
        //通过Addressable名字或者地址获得Handle和计数器
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

        //遍历释放所有Addressables资源
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
                 // 并非第一次加载
                if (assetInfos.TryGetValue(assetKey, out var assetInfo))
                {
                    assetInfo.RefCount++;
                    return (T)assetInfo.Handle.Result;
                }

                // 第一次加载
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetKey);
                await handle.Task;

                assetInfo = new AssetInfo
                {
                    Handle = handle,
                    RefCount = 1,
                };

                assetInfos[assetKey] = assetInfo;
                return handle.Result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"加载失败 {assetKey}: {ex.Message}");
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

        // 实例化GameObject，改对象池
        public async Task<GameObject> InstantiateAsync(string assetKey, Vector3 position, Quaternion rotation)
        {
            try
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetKey, position, rotation);
                GameObject instance = await handle.Task;
                return instance;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

        // 销毁实例化的对象，改对象池
        public void ReleaseInstance(GameObject instance)
        {
            if (instance != null)
            {
                Addressables.ReleaseInstance(instance);
            }
        }

        // 通过Label加载一堆资源
        // 释放一堆资源
    }
}