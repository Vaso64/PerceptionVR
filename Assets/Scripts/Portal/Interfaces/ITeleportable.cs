using System;
using PerceptionVR.Common;

namespace PerceptionVR.Portals
{
    public interface ITeleportable : IMonoBehaviour
    {
        Action<TeleportData> OnTeleport { get; set; }
        
        public bool manualTeleport { get; set; }
    }
}