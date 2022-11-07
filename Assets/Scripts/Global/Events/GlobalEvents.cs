using System;
using PerceptionVR.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PerceptionVR.Global
{
    public static class GlobalEvents
    {
        // Invoked by AddComponentNotify() in GameObjectExtensions
        public static Action<Component> OnAfterAddComponent;
        
        // Invoked by RemoveComponentNotify() in GameObjectExtensions
        public static Action<Component> OnBeforeRemoveComponent;
        
        // Invoked by InstantiateNotify() in ObjectExtensions
        public static Action<GameObject> OnAfterInstantiate;
        
        // Invoked by DestroyNotify() in ObjectExtensions
        public static Action<GameObject> OnBeforeDestroy;
    }
}