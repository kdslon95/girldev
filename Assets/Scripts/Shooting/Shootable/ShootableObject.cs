using UnityEngine;

namespace Shooting.Shootable
{
    public abstract class ShootableObject : MonoBehaviour
    {
        public enum ShootingFrequency
        {
            Single,
            Multiple
        }

        [SerializeField]
        private ShootingFrequency shootingFreq;
        public ShootingFrequency ShootingFreq => shootingFreq;
        
        [SerializeField, Min(0f)]
        private float cooldown;
        public float Cooldown => cooldown;
        
        [SerializeField, Min(0f)]
        private float damage;
        public float Damage => damage;

        //Modifiers
    }
}