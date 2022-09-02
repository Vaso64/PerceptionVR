using UnityEngine;

namespace PerceptionVR.Global
{
    public static class GameObjectUtility
    {
        // Empty wrapper for Instantiate and Destroy methods
        public static GameObject Instantiate(GameObject original) => Object.Instantiate(original);
        public static void Destroy(GameObject instance) => Object.Destroy(instance);
        
        
        // Instantiate and invoke OnAfterInstantiate in GlobalEvents
        public static Object InstantiateNotify(GameObject original)
        {
            var instance = Object.Instantiate(original);
            GlobalEvents.OnAfterInstantiate?.Invoke(instance);
            return instance;
        }
        
        
        // Invoke OnBeforeDestroy in GlobalEvents and Destroy
        public static void DestroyNotify(GameObject instance)
        {
            GlobalEvents.OnBeforeDestroy?.Invoke(instance);
            Object.Destroy(instance);
        }
    }
}