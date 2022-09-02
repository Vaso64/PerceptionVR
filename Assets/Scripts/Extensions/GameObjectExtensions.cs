using UnityEngine;
using PerceptionVR.Global;

namespace PerceptionVR.Extensions
{
    public static class GameObjectExtensions
    {
        // Remove components from a gameObject
        public static void RemoveComponent(this GameObject gameObject, Component component) => Object.Destroy(component);
        
        
        // Add component to a gameObject and invoke OnAfterAddComponent in GlobalEvents
        public static T AddComponentNotify<T>(this GameObject gameObject) where T : Component
        {
            var instance = gameObject.AddComponent<T>();
            GlobalEvents.OnAfterAddComponent?.Invoke(instance);
            return instance;
        }
        
        
        // Invoke OnBeforeRemoveComponent in GlobalEvents and remove component from a gameObject
        public static void RemoveComponentNotify(this GameObject gameObject, Component component)
        {
            GlobalEvents.OnBeforeRemoveComponent?.Invoke(component);
            gameObject.RemoveComponent(component);
        }
    }
}