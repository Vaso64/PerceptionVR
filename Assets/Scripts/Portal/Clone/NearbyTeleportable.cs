using UnityEngine;
using System.Collections.Generic;

namespace PerceptionVR.Portals
{
    [System.Serializable]
    public class NearbyTeleportable
    {
        public readonly TeleportableObject teleportableObject;
        public readonly TeleportableObjectClone cloneTeleportableObject;
        public List<Collider> collidersInVicinity;
        public List<Collider> cloneColliderInPairVicinity;
        
        public NearbyTeleportable(TeleportableObject teleportableObject, TeleportableObjectClone teleportableObjectClone)
        {
            this.teleportableObject = teleportableObject;
            this.cloneTeleportableObject = teleportableObjectClone;
            this.collidersInVicinity = new List<Collider>();
            this.cloneColliderInPairVicinity = new List<Collider>();
        }
    }

}