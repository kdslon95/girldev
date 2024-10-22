using UnityEngine;

namespace Shooting.Shootable
{
    [RequireComponent(typeof(Rigidbody))]
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

        [SerializeField, Min(0f)] 
        private float speed;
        
        //Modifiers

        private Rigidbody rb;
        public virtual void PrepareShootableObject()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        public virtual void StartMovement(Vector3 direction)
        {
            rb.AddForce(direction * speed, ForceMode.Impulse);
        }
    }
}