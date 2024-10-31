using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Shooting.Shootable;
using Subsystems.World;
using UnityEngine;

namespace Shooting
{
    public class BulletData
    {
        public ShootableTag bulletTag;
        public bool isReady;

        public BulletData(ShootableTag bulletTag, bool isReady)
        {
            this.bulletTag = bulletTag;
            this.isReady = isReady;
        }
    }

    public class ShootingComponent : MonoBehaviour
    {
        [SerializeField]
        private List<ShootableTag> availableBullets;
        private Dictionary<Guid, BulletData> bulletsTimerDictionary = new Dictionary<Guid, BulletData>();
        
        private Transform spawnSpot;
        private (Guid timerId, BulletData bulletData) currentBullet;
        
        private BulletFactorySubsystem bulletFactorySubsystem;
        
        private void Start()
        {
            bulletFactorySubsystem = GamePersistent.GetActiveWorld().GetSubsystem<BulletFactorySubsystem>();
            
            List<ShootableTag> distinctTags = availableBullets.Distinct().ToList();
            if (distinctTags.Count != availableBullets.Count)
            {
                Debug.LogError("Bullet tag must be unique. Duplicated tags has been removed from available bullets list.", this);
                availableBullets = distinctTags;
            }

            for (int i = 0; i < availableBullets.Count; ++i)
            {
                int idx = i;
                TimerSubsystem timerSubsystem = GamePersistent.GetActiveWorld().GetSubsystem<TimerSubsystem>();
                Guid timerId = timerSubsystem.SetTimer(bulletFactorySubsystem.CheckCooldown(availableBullets[i]), ()=>OnCooldownPassed(idx), 
                    true, true, true);
                bulletsTimerDictionary.Add(timerId, new BulletData(availableBullets[i], true));
            }
            
            Transform[] children = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.gameObject.CompareTag("SpawnSpot"))
                {
                    spawnSpot = child;
                    KeyValuePair<Guid, BulletData> firstBullet = bulletsTimerDictionary.First();
                    currentBullet = (firstBullet.Key, firstBullet.Value);
                    return;
                }
            }
            
            Debug.LogError($"Spawn Spot not found in children of {this.gameObject.name}", this.gameObject);
        }
        
        void Update()
        {
            bool shootingButtonUsed = Input.GetButtonDown("Fire"); //currentBullet.bulletData.prefab.ShootingFreq == ShootableObject.ShootingFrequency.Single ?
                //Input.GetButtonDown("Fire") : Input.GetButton("Fire");
            
            if (shootingButtonUsed)
            {
                if (currentBullet.bulletData.isReady)
                {
                    bulletFactorySubsystem.SpawnBullet(ShootableTag.Heavy, spawnSpot.position, spawnSpot.rotation);
                    GamePersistent.GetActiveWorld().GetSubsystem<TimerSubsystem>().ResumeTimer(currentBullet.timerId);
                    currentBullet.bulletData.isReady = false;
                }
            }
        }

        private void OnCooldownPassed(int id)
        {
            bulletsTimerDictionary.Values.ToArray()[id].isReady = true;
            Debug.Log("READY FOR SHOOT");
        }
    }
}