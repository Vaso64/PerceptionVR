using System;

namespace PerceptionVR.Portals
{
    public class TeleportableObject: MonoBehaviourBase, ISwappable
    {
        public Action<TeleportData> OnTeleport { get; set; }
        public Action<SwapData> OnSwap { get; set; }
        public bool manualTeleport;
    }
}