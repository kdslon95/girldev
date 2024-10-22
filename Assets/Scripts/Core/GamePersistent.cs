using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class GamePersistent
    {
        private static Dictionary<Type, PersistentSubsystem> registeredSubsystems;
        private static World world;
        private static Scene activeScene;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void OnAppStarted()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            
            IEnumerable<Type> subsystemTypes = Assembly.GetExecutingAssembly().GetTypes().
                Where(t => t.IsSubclassOf(typeof(PersistentSubsystem)) && !t.IsAbstract);

            foreach (Type type in subsystemTypes)
            {
                PersistentSubsystem subsystem = (PersistentSubsystem)Activator.CreateInstance(type);
                subsystem.InitializeSubsystem();
                registeredSubsystems.Add(type, subsystem);
            }
        }
        
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Single) 
                return;
            
            activeScene = scene;
            SpawnNewWorld();
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if (scene == activeScene)
            {
                activeScene = SceneManager.GetActiveScene();
                SpawnNewWorld();
            }
        }

        private static void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            activeScene = newScene;
            SpawnNewWorld();
        }
        
        private static void OnProcessExit(object sender, EventArgs e)
        {
            foreach (PersistentSubsystem subsystem in registeredSubsystems.Values)
            {
                subsystem.DisposeSubsystem();    
            }
        }

        private static void SpawnNewWorld()
        {
            if (world != null)
            {
                world.InvalidateWorld();
            }
            
            GameObject worldObject = new GameObject("World");
            worldObject.transform.SetAsFirstSibling();
            world = worldObject.AddComponent<World>();
            world.PrepareWorld();
        }

        public static TSubsystem GetSubsystem<TSubsystem>() where TSubsystem : PersistentSubsystem
        {
            if (registeredSubsystems.ContainsKey(typeof(TSubsystem)))
            {
                return (TSubsystem)registeredSubsystems[typeof(TSubsystem)];
            }

            return null;
        }

        public static World GetActiveWorld()
        {
            return world;
        }
    }
}