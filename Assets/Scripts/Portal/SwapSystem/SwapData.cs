using System.Collections.Generic;
using PerceptionVR.Common;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public struct SwapData
    {
        public readonly (ITeleportable, ITeleportable) teleportableSwap;
        public readonly IEnumerable<(Collider, Collider)> colliderSwaps;
        
        public SwapData(ITeleportable teleportable, ITeleportable clone)
        {
            teleportableSwap = (teleportable, clone);
            colliderSwaps = ComponentUtility.CreateComponentTuple<Collider, Collider>(teleportable.transform, clone.transform);
        }
    }
}