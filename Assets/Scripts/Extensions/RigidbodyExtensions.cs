using UnityEngine;


namespace PerceptionVR.Extensions
{
    public static class RigidbodyExtensions
    {
            // Moves rigidbody towards position using velocity
            public static void VelocityMove(this Rigidbody rb, Vector3 position, bool isLocalPosition = false)
            {
                rb.velocity = (position - (isLocalPosition ? rb.transform.localPosition : rb.transform.position))
                              / Time.fixedDeltaTime;
            }

            // Rotates rigidbody towards rotation sing angular velocity
            public static void VelocityRotate(this Rigidbody rb, Quaternion rotation, bool isLocalRotation = false)
            {
                var result = (rotation * Quaternion.Inverse(isLocalRotation ? rb.transform.localRotation : rb.transform.rotation))
                             .GetClampedEulerAngles()
                             .Deg2Rad()
                             / Time.fixedDeltaTime;
                rb.angularVelocity = result;
            }

            // Positions rigidbody towards pose using velocity and angular velocity
            public static void VelocityPosition(this Rigidbody rb, Pose pose, bool isLocalPose = false)
            {
                rb.VelocityMove(pose.position, isLocalPose);
                rb.VelocityRotate(pose.rotation, isLocalPose);
            }
        }
}
