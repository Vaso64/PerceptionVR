using UnityEngine;

namespace PerceptionVR.Portals
{
    public struct TeleportData
    {
        public TeleportableObject teleportableObject;
        public Portal inPortal;
        public Portal outPortal;
        public Quaternion rotationDelta;
    }
}