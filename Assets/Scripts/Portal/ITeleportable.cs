using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface ITeleportable
    {
        void OnTeleport(TeleportData teleportData);
    }
}