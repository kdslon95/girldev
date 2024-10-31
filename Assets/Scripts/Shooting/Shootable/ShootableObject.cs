using UnityEngine;
using Utils;

namespace Shooting.Shootable
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ShootableObject : MonoBehaviour, IPoolableObject
    {
        public enum ShootingFrequency
        {
            Single,
            Multiple
        }

        [SerializeField] 
        private ShootableTag shootableTag;
        public ShootableTag ShootableTag => shootableTag;
        
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

        public virtual void PrepareForSpawn()
        {
            if(rb == null)
                PrepareShootableObject();

            StartMovement(Vector3.forward);
        }

        public virtual void PrepareForDespawn()
        {
            if(rb != null)
                rb.velocity = Vector3.zero;
        }
    }
}