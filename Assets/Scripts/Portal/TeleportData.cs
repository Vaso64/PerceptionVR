using UnityEngine;

namespace PerceptionVR.Portal
{
    public struct TeleportData
    {
        public ITeleportable teleportable;
        public TeleportableClone teleportableClone;
        public IPortal inPortal;
        public IPortal outPortal;
        public Quaternion rotationDelta;
        public SwapData swapData;
        
        public TeleportData(ITeleportable teleportable, TeleportableClone teleportableClone, IPortal inPortal)
        {
            this.teleportable = teleportable;
            this.teleportableClone = teleportableClone;
            this.inPortal = inPortal;
            this.outPortal = inPortal.portalPair;
            this.rotationDelta = Quaternion.identity;
            this.swapData = new SwapData(teleportable, teleportableClone);
        }
    }
}