using UnityEngine;

namespace PerceptionVR.Portal
{
    public struct TeleportData
    {
        public ITeleportable teleportable;
        public Portal inPortal;
        public Portal outPortal;
        public Quaternion rotationDelta;
    }
}