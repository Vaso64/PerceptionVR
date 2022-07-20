using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface IPortal
    {
        float PortalDot(Transform transform);
        
        Pose PairPose(Pose pose);
        
        Plane portalPlane { get; }
        
        void Teleport(NearbyObject teleportable);
    }
}