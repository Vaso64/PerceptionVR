using System.Collections.Generic;
using PerceptionVR.Common;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public struct SwapData
    {
        public readonly (TeleportableObject, TeleportableObjectClone) teleportableSwap;
        public readonly IEnumerable<(Collider, Collider)> colliderSwaps;
        public readonly IEnumerable<(Renderer, Renderer)> rendererSwaps;
        
        public SwapData(TeleportableObject teleportableObject, TeleportableObjectClone clone)
        {
            teleportableSwap = (teleportableObject, clone);
            colliderSwaps = ComponentUtility.CreateComponentTuple<Collider, Collider>(teleportableObject.transform, clone.transform);
            rendererSwaps = ComponentUtility.CreateComponentTuple<Renderer, Renderer>(teleportableObject.transform, clone.transform);
        }
    }
}