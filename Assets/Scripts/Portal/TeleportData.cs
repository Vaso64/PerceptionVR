using UnityEngine;

namespace PerceptionVR.Portals
{
    public struct TeleportData
    {
        public ITeleportable teleportable;
        public Portal inPortal;
        public Portal outPortal;
        public Quaternion rotationDelta;
    }
}