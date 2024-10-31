using System.Collections.Generic;
using Core;
using Shooting;
using Shooting.Shootable;
using UnityEngine;
using Utils;

namespace Subsystems.World
{
    public class BulletFactorySubsystem : WorldSubsystem
    {
        private Dictionary<ShootableTag, MonoPool<ShootableObject>> poolsDictionary;
        
        public override void InitializeSubsystem()
        {
            poolsDictionary = new Dictionary<ShootableTag, MonoPool<ShootableObject>>();
            
            ShootableSet set = Resources.Load<ShootableSet>("Data/EasyBulletSet");
            foreach (ShootableObject shootableObject in set.AvailableShootableObjects)
            {
                if (poolsDictionary.ContainsKey(shootableObject.ShootableTag))
                {
                    Debug.LogError($"The pool with tag {shootableObject.ShootableTag} already exists. Only one prefab per tag is allowed! Skipping.");
                    continue;
                }

                MonoPool<ShootableObject> newPool = new MonoPool<ShootableObject>(shootableObject, 10);
                poolsDictionary.Add(shootableObject.ShootableTag, newPool);
            }
        }

        public ShootableObject SpawnBullet(ShootableTag shootableTag, Vector3 position, Quaternion rotation)
        {
            ShootableObject shootableObject = poolsDictionary[shootableTag].GetFromPool();
            shootableObject.transform.SetPositionAndRotation(position, rotation);
            return shootableObject;
        }

        public void DespawnBullet(ShootableObject bullet)
        {
            poolsDictionary[bullet.ShootableTag].ReturnToPool(bullet);
        }

        public float CheckCooldown(ShootableTag shootableTag)
        {
            return poolsDictionary[shootableTag].OriginalPrefab.Cooldown;
        }

        public override void DisposeSubsystem()
        {
            
        }
    }
}