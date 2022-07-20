using UnityEngine;

namespace PerceptionVR.Portal
{
    public struct TeleportData
    {
        public TeleportData(Quaternion portalDelta)
        {
            this.portalDelta = portalDelta;
        }
        
        public Quaternion portalDelta;
    }
}