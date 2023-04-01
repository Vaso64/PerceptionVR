using System;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public class TeleportableObject: MonoBehaviourBase
    {
        public Action<TeleportData> OnTeleport { get; set; }
        public bool manualTeleport { get; set; }
    }
}