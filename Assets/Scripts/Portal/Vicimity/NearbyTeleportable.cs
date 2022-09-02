using System;
using UnityEngine;
using System.Collections.Generic;

namespace PerceptionVR.Portal
{
    public record NearbyTeleportable
    {
        public NearbyTeleportable(ITeleportable teleportable, TeleportableClone clone)
        {
            this.teleportable = teleportable;
            this.clone = clone;
            this.associatedColliders = new List<Collider>();
        }
        public ITeleportable teleportable;
        public readonly TeleportableClone clone;
        public readonly ICollection<Collider> associatedColliders;
    }

}