using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface ITeleportable
    {
        public Transform transform { get; }
        
        void OnTeleport(TeleportData teleportData);
    }
}