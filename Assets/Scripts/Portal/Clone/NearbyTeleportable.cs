using UnityEngine;
using System.Collections.Generic;

namespace PerceptionVR.Portal
{
    [System.Serializable]
    public class NearbyTeleportable
    {
        public readonly ITeleportable teleportable;
        public readonly ITeleportable cloneTeleportable;
        public List<Collider> collidersInVicinity;
        public List<Collider> cloneColliderInPairVicinity;
        
        public NearbyTeleportable(ITeleportable teleportable, ITeleportable cloneTeleportable)
        {
            this.teleportable = teleportable;
            this.cloneTeleportable = cloneTeleportable;
            this.collidersInVicinity = new List<Collider>();
            this.cloneColliderInPairVicinity = new List<Collider>();
        }
    }

}