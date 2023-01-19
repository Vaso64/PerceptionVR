using System;
using PerceptionVR.Common;

namespace PerceptionVR.Portal
{
    public interface ITeleportable : IMonoBehaviour
    {
        Action<TeleportData> OnTeleport { get; set; }
        
        public bool manualTeleport { get; set; }
    }
}