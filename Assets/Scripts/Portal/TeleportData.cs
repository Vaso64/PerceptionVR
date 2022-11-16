using UnityEngine;

namespace PerceptionVR.Portal
{
    public struct TeleportData
    {
        public ITeleportable teleportable;
        public TeleportableClone clone;
        public IPortal inPortal;
        public IPortal outPortal;
        public Quaternion portalDelta;
    }
}