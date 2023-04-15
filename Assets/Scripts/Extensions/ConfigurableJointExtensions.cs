using UnityEngine;

namespace PerceptionVR.Extensions
{
    public static class ConfigurableJointExtensions
    {
        public static void SetDrive(this ConfigurableJoint joint, float positionSpring, float positionDamper, float maximumForce = Mathf.Infinity)
        {
            var drive = new JointDrive {positionSpring = positionSpring, positionDamper = positionDamper, maximumForce = maximumForce};
            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;
        }
        
        public static void SetAngularDrive(this ConfigurableJoint joint, float positionSpring, float positionDamper, float maximumForce = Mathf.Infinity)
        {
            var drive = new JointDrive {positionSpring = positionSpring, positionDamper = positionDamper, maximumForce = maximumForce};
            joint.angularXDrive = drive;
            joint.angularYZDrive = drive;
        }
    }
}