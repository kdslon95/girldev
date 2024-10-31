using System;
using Core;
using Shooting.Shootable;
using Subsystems.Persistent;
using Subsystems.World;
using UnityEngine;

namespace Shooting
{
    public class ShootingComponent : MonoBehaviour
    {
        private (Guid timerId, ShootableType type) lightBullet;
        private (Guid timerId, ShootableType type) heavyBullet;
        private (Guid timerId, ShootableType type)[] bullets;
        
        private int currentBullet;
        private Transform spawnSpot;
        
        private BulletFactorySubsystem bulletFactorySubsystem;
        private InputSubsystem inputSubsystem;
        private TimerSubsystem timerSubsystem;
        
        private void Start()
        {
            bulletFactorySubsystem = GamePersistent.GetActiveWorld().GetSubsystem<BulletFactorySubsystem>();
            inputSubsystem = GamePersistent.GetActiveWorld().GetSubsystem<InputSubsystem>();
            timerSubsystem = GamePersistent.GetActiveWorld().GetSubsystem<TimerSubsystem>();
            
            #region BULLETS_INITIALIZATION
            float lightBulletCooldown = bulletFactorySubsystem.GetCooldown(ShootableType.Light);
            Guid lightTimerId = timerSubsystem.SetTimer(lightBulletCooldown, null, 
                true, true, true);
            
            lightBullet = (lightTimerId, ShootableType.Light);
           
            float heavyBulletCooldown = bulletFactorySubsystem.GetCooldown(ShootableType.Heavy);
            Guid heavyTimerId = timerSubsystem.SetTimer(heavyBulletCooldown, null, 
                true, true, true);
            
            heavyBullet = (heavyTimerId, ShootableType.Heavy);

            bullets = new[] { lightBullet, heavyBullet };
            currentBullet = 0;

            #endregion

            Transform[] children = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.gameObject.CompareTag("SpawnSpot"))
                {
                    spawnSpot = child;
                    return;
                }
            }

            Debug.LogError($"Spawn Spot not found in children of {this.gameObject.name}", this.gameObject);
        }
        
        void Update()
        {
            if (inputSubsystem.InputContext.isWeapon1Down)
            {
                currentBullet = 0;
            }
            else if (inputSubsystem.InputContext.isWeapon2Down)
            {
                currentBullet = 1;
            }
            
            if (inputSubsystem.InputContext.isFireDown && timerSubsystem.IsTimerPaused(bullets[currentBullet].timerId))
            {
                bulletFactorySubsystem.SpawnBullet(bullets[currentBullet].type, spawnSpot.position, spawnSpot.rotation);
                GamePersistent.GetActiveWorld().GetSubsystem<TimerSubsystem>().ResumeTimer(bullets[currentBullet].timerId);
            }
        }
    }
}