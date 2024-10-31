using System;
using Shooting.Shootable;
using UnityEngine;

namespace Shooting
{
    [CreateAssetMenu(fileName = "ShootableSet", menuName = "Data/New Shootable Set", order = 1)]
    public class ShootableSet : ScriptableObject
    {
        [SerializeField]
        private ShootableObject[] availableShootableObjects;
        public ShootableObject[] AvailableShootableObjects => availableShootableObjects;
    }
}