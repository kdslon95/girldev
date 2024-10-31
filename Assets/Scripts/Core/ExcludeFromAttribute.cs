using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeFromAttribute : Attribute
    {
        private int[] excludedScenes;
        
        public ExcludeFromAttribute(params int[] sceneIdx)
        {
            excludedScenes = sceneIdx;
        }

        public bool IsSceneExcluded(int sceneIdx)
        {
            foreach (int excludedScene in excludedScenes)
            {
                if (excludedScene == sceneIdx)
                    return true;
            }
            return false;
        }
    }
}