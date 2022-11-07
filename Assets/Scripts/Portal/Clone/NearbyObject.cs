using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PerceptionVR.Portal
{
    public record NearbyObject
    {
        public NearbyObject(ITeleportable teleportable, TeleportableClone clone)
        {
            this.teleportable = teleportable;
            this.associatedColliders = teleportable.transform.GetComponentsInChildren<Collider>();
            this.collidersInVicinity = new List<Collider>();
            this.clone = clone;
            this.associatedCloneColliders = clone.GetComponentsInChildren<Collider>();
            this.cloneCollidersInVicinity = new List<Collider>();
            this.pairLookup = new();
            foreach (var pair in associatedColliders.Zip(associatedCloneColliders, (original, clone) => (original, clone)))
                this.pairLookup.Add((pair.original, pair.clone));
        }
        public readonly ITeleportable teleportable;
        public readonly IEnumerable<Collider> associatedColliders;
        public List<Collider> collidersInVicinity;
        public readonly TeleportableClone clone;
        public readonly IEnumerable<Collider> associatedCloneColliders;
        public List<Collider> cloneCollidersInVicinity;
        public readonly List<(Collider original, Collider clone)> pairLookup;
    }

}