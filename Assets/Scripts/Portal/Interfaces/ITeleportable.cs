using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PerceptionVR.Portal
{
    public interface ITeleportable : ITeleportableBase
    {
        void OnTeleport(TeleportData teleportData) {}
    }
}