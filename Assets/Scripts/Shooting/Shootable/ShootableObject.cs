using System;
using UnityEngine;
using UnityEngine.Events;
using Utils.ObjectPooling;

namespace Shooting.Shootable
{
    public enum ShootableType
    {
        Light,
        Heavy,
    }
    
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ShootableObject : MonoBehaviour, IPoolableObject
    {
        [SerializeField]
        private ShootableType shootableType;
        public ShootableType ShootType => shootableType;
        
        [SerializeField, Min(0f)]
        private float cooldown;
        public float Cooldown => cooldown;
        
        [SerializeField, Min(0f)]
        private float damage;
        public float Damage => damage;

        [SerializeField, Min(0f)] 
        private float speed;
        public float Speed => speed;
        
        //TODO: Modifiers

        //TODO: Make it private?
        public UnityAction<ShootableObject> OnHitEvent;

        private Rigidbody rb;
        protected virtual void PrepareShootableObject()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }

        public virtual void StartMovement(Vector3 direction)
        {
            rb.AddForce(direction * speed, ForceMode.Impulse);
        }

        public void PrepareForSpawn(Vector3 position, Quaternion rotation)
        {
            if(rb == null)
                PrepareShootableObject();

            StartMovement(Vector3.forward);
        }

        public virtual void PrepareForDespawn()
        {
            OnHitEvent = null;
            rb.velocity = Vector3.zero;
        }

        public void OnCollisionEnter(Collision other)
        {
            OnHitEvent?.Invoke(this);
        }
    }
}