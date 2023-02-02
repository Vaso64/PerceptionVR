using System;
using PerceptionVR.Portals;
using UnityEngine;

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
        
        // Invoked by Teleport() in PortalBase
        public static Action<TeleportData> OnTeleport;
        
        // Invoked in PortalCloneSystem
        public static Action<SwapData> OnSwap;
    }
}