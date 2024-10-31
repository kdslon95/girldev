using System;
using Shooting.Shootable;
using UnityEngine;

namespace Shooting
{
    [Serializable]
    public class CollectionData
    {
        public ShootableObject shootableObject;
        public int size;
    }

    [CreateAssetMenu(fileName = "ShootableCollection", menuName = "Data/New Shootable Collection", order = 1)]
    public class ShootableCollection : ScriptableObject
    {
        [SerializeField] 
        private CollectionData lightBullet;
        public CollectionData LightBullet => lightBullet;
        
        [SerializeField] 
        private CollectionData heavyBullet;
        public CollectionData HeavyBullet => heavyBullet;
    }
}