using PerceptionVR.Common;

namespace PerceptionVR.Portal
{
    public interface ITeleportable : IMonoBehaviour
    {
        void OnTeleport(TeleportData teleportData) {}
    }
}