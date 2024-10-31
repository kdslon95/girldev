using System.Collections.Generic;
using Core;
using Shooting;
using Shooting.Shootable;
using UnityEngine;
using Utils.ObjectPooling;

namespace Subsystems.World
{
    [ExcludeFrom(SceneID.MainMenu)]
    public class BulletFactorySubsystem : WorldSubsystem
    {
        private Dictionary<ShootableType, MonoPool<ShootableObject>> poolsDictionary;
        
        public override void InitializeSubsystem()
        {
            poolsDictionary = new Dictionary<ShootableType, MonoPool<ShootableObject>>();
            
            //TODO: Load set based on game difficulty
            ShootableCollection collection = Resources.Load<ShootableCollection>("Data/0_EasyCollection");
            
            MonoPool<ShootableObject> lightBulletPool = new MonoPool<ShootableObject>(collection.LightBullet.shootableObject, 
                collection.LightBullet.size);
            MonoPool<ShootableObject> heavyBulletPool = new MonoPool<ShootableObject>(collection.HeavyBullet.shootableObject, 
                collection.HeavyBullet.size);
            
            poolsDictionary.Add(ShootableType.Light, lightBulletPool);
            poolsDictionary.Add(ShootableType.Heavy, heavyBulletPool);
        }

        public ShootableObject SpawnBullet(ShootableType shootableType, Vector3 position, Quaternion rotation)
        {
            return poolsDictionary[shootableType].GetFromPool(position, rotation);
        }

        public void DespawnBullet(ShootableObject bullet)
        {
            poolsDictionary[bullet.ShootType].ReturnToPool(bullet);
        }

        public float GetCooldown(ShootableType shootableType)
        {
            return poolsDictionary[shootableType].TemplatePrefab.Cooldown;
        }
        
        public float GetSpeed(ShootableType shootableType)
        {
            return poolsDictionary[shootableType].TemplatePrefab.Speed;
        }
        
        public float GetDamage(ShootableType shootableType)
        {
            return poolsDictionary[shootableType].TemplatePrefab.Damage;
        }

        public override void DisposeSubsystem()
        {
            //TODO: Despawn pools
        }
    }
}