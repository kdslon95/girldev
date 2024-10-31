using UnityEngine;

namespace Utils.ObjectPooling
{
    public interface IPoolableObject
    {
        void PrepareForSpawn(Vector3 position, Quaternion rotation);
        void PrepareForDespawn();
    }
}