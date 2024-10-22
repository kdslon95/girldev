using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core
{
    public class World : MonoBehaviour
    {
        private Dictionary<Type, WorldSubsystem> registeredSubsystems;
        
        public void PrepareWorld()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            registeredSubsystems = new Dictionary<Type, WorldSubsystem>();
            IEnumerable<Type> subsystemTypes = Assembly.GetExecutingAssembly().GetTypes().
                Where(t => t.IsSubclassOf(typeof(WorldSubsystem)) && !t.IsAbstract);

            foreach (Type type in subsystemTypes)
            {
                WorldSubsystem subsystem = (WorldSubsystem)Activator.CreateInstance(type);
                subsystem.InitializeSubsystem();
                registeredSubsystems.Add(type, subsystem);
            }
        }

        private void Update()
        {
            foreach (WorldSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.TickSubsystem(Time.deltaTime);
            }
        }

        public void InvalidateWorld()
        {
            foreach (WorldSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.DisposeSubsystem();    
                Destroy(this.gameObject);
            }
        }
        
        public TSubsystem GetSubsystem<TSubsystem>() where TSubsystem : WorldSubsystem
        {
            if (registeredSubsystems.ContainsKey(typeof(TSubsystem)))
            {
                return (TSubsystem)registeredSubsystems[typeof(TSubsystem)];
            }

            return null;
        }
    }
}