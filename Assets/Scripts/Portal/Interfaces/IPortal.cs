using System;
using PerceptionVR.Common;
using UnityEngine;

namespace PerceptionVR.Portal
{
    public interface IPortal : IMonoBehaviour
    {
        IPortal portalPair { get; }

        Pose PairPose(Pose pose);
        
        Collider portalCollider { get; }
        
        Plane portalPlane { get; }

        Action<ITeleportable> OnTeleport { get; set; }
        
        void Teleport(ITeleportable teleportable);
    }
}