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
        public ShootableObject prefab;
        public bool isReady;

        public BulletData(ShootableObject prefab, bool isReady)
        {
            this.prefab = prefab;
            this.isReady = isReady;
        }
    }

    public class ShootingComponent : MonoBehaviour
    {
        [SerializeField]
        private ShootableObject[] availableBullets;
        private Dictionary<Guid, BulletData> bulletsTimerDictionary = new Dictionary<Guid, BulletData>();
        
        private Transform spawnSpot;
        private (Guid timerId, BulletData bulletData) currentBullet;
        
        private void Start()
        {
            for (int i = 0; i < availableBullets.Length; ++i)
            {
                int idx = i;
                TimerSubsystem timerSubsystem = GamePersistent.GetActiveWorld().GetSubsystem<TimerSubsystem>();
                Guid timerId = timerSubsystem.SetTimer(availableBullets[i].Cooldown, ()=>OnCooldownPassed(idx), 
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
            bool shootingButtonUsed = currentBullet.bulletData.prefab.ShootingFreq == ShootableObject.ShootingFrequency.Single ?
                Input.GetButtonDown("Fire") : Input.GetButton("Fire");
            
            if (shootingButtonUsed)
            {
                if (currentBullet.bulletData.isReady)
                {
                    Instantiate(currentBullet.bulletData.prefab, spawnSpot.position, spawnSpot.rotation);
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