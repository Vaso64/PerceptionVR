using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Common;

namespace PerceptionVR.Portal
{
    public interface ITeleportable : IMonoBehaviour
    {
        void OnTeleport(TeleportData teleportData) {}
    }
}