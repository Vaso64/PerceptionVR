using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface IPortal
    {
        Transform transform { get; }
        
        IPortal portalPair { get; }

        Pose PairPose(Pose pose);
        
        Bounds teleportableBounds { get; }
        
        Plane portalPlane { get; }
        
        void Teleport(ITeleportable teleportable);
    }
}