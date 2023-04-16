using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Extensions
{
    public static class ComponentExtensions
    {
        public static bool TryGetComponentInParent<T>(this Component component, out T result)
        {
            result = component.GetComponentInParent<T>();
            return result != null;
        }
        
        public static bool TryGetComponentInChildren<T>(this Component component, out T result)
        {
            result = component.GetComponentInChildren<T>();
            return result != null;
        }
    }
}