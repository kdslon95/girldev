using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Utils
{
    public class MonoPool<T>
        where T : MonoBehaviour, IPoolableObject
    {
        private GameObject poolParent;
        private T originalPrefab;
        private int size;
        private Stack<T> poolStack;

        public T OriginalPrefab => originalPrefab;

        public MonoPool(T originalPrefab, int size)
        {
            poolStack = new Stack<T>();
            this.originalPrefab = originalPrefab;
            this.size = size;

            World world = GamePersistent.GetActiveWorld();
            poolParent = world.SpawnLabelObject(originalPrefab.GetType().Name + "Pool");
            for (int i = 0; i < size; i++)
            {
                T spawnedObject = world.SpawnObject(originalPrefab);
                ReturnToPool(spawnedObject);
            }
        }

        public T GetFromPool()
        {
            if (poolStack.Count > 0)
            {
                T obj = poolStack.Pop();
                obj.transform.SetParent(null);
                obj.gameObject.SetActive(true);
                obj.PrepareForSpawn();
                return obj;
            }

            Debug.LogError("Pool is empty. New object has been created. Consider extending the pool size");
            
            T newObj = GamePersistent.GetActiveWorld().SpawnObject(originalPrefab);
            newObj.gameObject.SetActive(true);
            newObj.PrepareForSpawn();
            return newObj;
        }

        public void ReturnToPool(T obj)
        {
            if (poolStack.Count < size)
            {
                obj.PrepareForDespawn();
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(poolParent.transform);
                poolStack.Push(obj);
                return;
            }
            
            Debug.LogError("Pool is full. Object has been destroyed. The number of the objects in the scene is too large. Please debug!");
            GamePersistent.GetActiveWorld().DespawnObject(obj);
        }
    }
}