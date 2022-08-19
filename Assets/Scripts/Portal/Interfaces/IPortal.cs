using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface IPortal
    {
        Transform transform { get; }
        
        IPortal portalPair { get; }

        Pose PairPose(Pose pose);
        
        Collider portalCollider { get; }
        
        Plane portalPlane { get; }
        
        void Teleport(ITeleportable teleportable);
    }
}