using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiaoXu.Core
{
    public enum AssetType
    {
        Prefab = 1,
        Sprite = 2
    }
    public enum PoolState
    {
        Use,
        Idle
    }
    public class PoolItem<T>
    {
        public T obj;
        public bool isClone; //物体末尾是否带克隆
        public PoolState poolState { set; get;  } // 物体状态
        
    }
    public class ObjectPool<T> where T : UnityEngine.Object
    {
        private List<PoolItem<T>> itemList = new List<PoolItem<T>>(); //池中现有物体
        //从池中获取物体
        public T GetItemFromPool(string assetKey)
        {
            T obj = null;
            
            return obj;
        }
        //创建物体到池中
        public T CreateItemToPool(T obj, out string assetKey)
        {
            T obj = null;

            return obj;
        }
        //回收物体到池中
        public void RemoveItemFromPool(T obj)
        {
            for(int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].obj.Equals(obj))
                {
                    itemList[i].poolState = PoolState.Idle;
                    break;
                }
            }
        }
    }
}

