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
        //通过Addressable名字或者地址获得Handle
        private Dictionary<string, AsyncOperationHandle> _handle = new Dictionary<string, AsyncOperationHandle>();

        public override void OnInit() { }
        public override void OnUpdate() { }
        public override void OnFixedUpdate() { }
        public override void OnLateUpdate() { }

        //遍历Release所有Addressables资源
        public override void OnDispose()
        {
            foreach (var handle in _handle.Values)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
            _handle.Clear();
        }

        // 加载资源的接口
        public async Task<T> LoadAssetAsync<T>(string assetKey) where T : UnityEngine.Object
        {
            try
            {
                // 如果已经加载过，直接返回
                if (_handle.ContainsKey(assetKey) && _handle[assetKey].IsValid())
                {
                    return (T)_handle[assetKey].Result;
                }

                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetKey);
                await handle.Task;
                _handle[assetKey] = handle;

                Debug.Log($"加载完成: {assetKey}");
                return handle.Result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"资源加载失败 {assetKey}: {ex.Message}");
                return null;
            }
        }

        // 释放单个资源
        public void ReleaseAsset(string assetKey)
        {
            if (_handle.ContainsKey(assetKey))
            {
                if (_handle[assetKey].IsValid())
                {
                    Addressables.Release(_handle[assetKey]);
                }
                _handle.Remove(assetKey);
                Debug.Log($"释放资源: {assetKey}");
            }
        }

        // 实例化 GameObject
        public async Task<GameObject> InstantiateAsync(string assetKey, Vector3 position, Quaternion rotation)
        {
            try
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetKey, position, rotation);
                GameObject instance = await handle.Task;

                Debug.Log($"实例化完成: {assetKey}");
                return instance;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"实例化失败 {assetKey}: {ex.Message}");
                return null;
            }
        }

        // 销毁实例化的对象
        public void ReleaseInstance(GameObject instance)
        {
            if (instance != null)
            {
                Addressables.ReleaseInstance(instance);
                Debug.Log($"释放实例: {instance.name}");
            }
        }
    }
}