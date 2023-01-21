using UnityEngine;
using Object = UnityEngine.Object;

namespace PerceptionVR.Global
{
    public static class GameObjectUtility
    {
        // Empty wrapper for Instantiate and Destroy methods
        public static GameObject Instantiate(GameObject original) => Object.Instantiate(original);
        public static void Destroy(GameObject instance) => Object.Destroy(instance);
        public static void DestroyComponent(Component component) => Object.Destroy(component);


        // Instantiate and invoke OnAfterInstantiate in GlobalEvents
        public static GameObject InstantiateNotify(GameObject original)
        {
            var instance = Object.Instantiate(original);
            GlobalEvents.OnAfterInstantiate?.Invoke(instance);
            return instance;
        }
        
        
        // Invoke OnBeforeDestroy in GlobalEvents and Destroys the GameObject
        public static void DestroyNotify(GameObject instance)
        {
            GlobalEvents.OnBeforeDestroy?.Invoke(instance);
            Object.Destroy(instance);
        }
        
        // Invoke OnBeforeRemoveComponent in GlobalEvents and Destroys the component
        public static void DestroyComponentNotify(Component component)
        {
            GlobalEvents.OnBeforeRemoveComponent?.Invoke(component);
            Object.Destroy(component);
        }
        
        // Invoke OnBeforeRemoveComponent in GlobalEvents and Destroys the component
        public static void DestroyComponentImmediateNotify(Component component)
        {
            GlobalEvents.OnBeforeRemoveComponent?.Invoke(component);
            Object.DestroyImmediate(component);
        }
    }
}