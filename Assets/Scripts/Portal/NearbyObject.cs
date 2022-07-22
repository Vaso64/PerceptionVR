using UnityEngine;

namespace PerceptionVR.Portal
{
    public record NearbyObject
    {
        public NearbyObject(ITeleportable teleportable, Transform transform, float portalDot)
        {
            this.teleportable = teleportable;
            this.portalDot = portalDot;
        }
        public ITeleportable teleportable;
        public float portalDot;
    }

}