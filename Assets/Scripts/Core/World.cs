using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public sealed class World : MonoBehaviour
    {
        private Dictionary<Type, WorldSubsystem> registeredSubsystems;

        public void PrepareWorld()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            registeredSubsystems = new Dictionary<Type, WorldSubsystem>();

            IEnumerable<Type> subsystemTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(WorldSubsystem)) && !t.IsAbstract);

            foreach (Type type in subsystemTypes)
            {
                ExcludeFromAttribute excludeAtt = type.GetCustomAttribute<ExcludeFromAttribute>();
                if (excludeAtt != null)
                {
                    if(excludeAtt.IsSceneExcluded(SceneManager.GetActiveScene().buildIndex))
                        continue;
                }
                
                WorldSubsystem subsystem = (WorldSubsystem)Activator.CreateInstance(type);
                subsystem.InitializeSubsystem();
                registeredSubsystems.Add(type, subsystem);
            }
        }
        
        private void Update()
        {
            foreach (WorldSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.PreTickSubsystem(Time.deltaTime);
            }
            
            foreach (WorldSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.TickSubsystem(Time.deltaTime);
            }
        }
        
        private void LateUpdate()
        {
            foreach (WorldSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.LateTickSubsystem(Time.deltaTime);
            }
        }

        public void InvalidateWorld()
        {
            foreach (WorldSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.DisposeSubsystem();
            }
            
            registeredSubsystems.Clear();
            Destroy(this.gameObject);
        }

        public TSubsystem GetSubsystem<TSubsystem>() 
            where TSubsystem : WorldSubsystem
        {
            if (registeredSubsystems.ContainsKey(typeof(TSubsystem)))
            {
                return (TSubsystem) registeredSubsystems[typeof(TSubsystem)];
            }

            return null;
        }
        
        public TItem SpawnObject<TItem>(TItem prefab, Transform parent = null) 
            where TItem : MonoBehaviour
        {
            TItem spawnedPrefab = Instantiate(prefab, parent);
            return spawnedPrefab;
        }
        
        public TItem SpawnObject<TItem>(TItem prefab, Vector3 position, Quaternion rotation, Transform parent = null) 
            where TItem : MonoBehaviour
        {
            TItem spawnedPrefab = Instantiate(prefab, position, rotation, parent);
            return spawnedPrefab;
        }

        public void DespawnObject(MonoBehaviour objectToDespawn, float timeToDespawn = 0f)
        {
            Destroy(objectToDespawn, timeToDespawn);            
        }

        public GameObject SpawnFolderObject(string label)
        {
            GameObject obj = new GameObject(label);
            obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            return obj;
        }
    }
}