using UnityEngine;
using PerceptionVR.Portal;

namespace PerceptionVR.Extensions
{
    public static class PortalExtensions
    {
        public static Pose PairPose(this Portal.Portal portal, Pose pose) => portal.PairPose(pose, out _);

        public static Pose PairPose(this Portal.Portal portal, Pose pose, out Quaternion portalRotationDelta)
        {
            Pose resultPose;
            
            portalRotationDelta = portal.portalPair.transform.rotation * Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(portal.transform.rotation);

            // Calculate position
            resultPose.position = portal.portalPair.transform.position + portalRotationDelta * (pose.position - portal.transform.position);

            // Rotation rotation
            resultPose.rotation = portalRotationDelta * pose.rotation;

            return resultPose;
        }
    }
}