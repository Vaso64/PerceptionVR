using UnityEngine;
using PerceptionVR.Extensions;
using UnityEngine.ProBuilder;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(VRPlayerInput))]
    public class VRPlayer : PlayerBase
    {
        private VRPlayerInput vrInput;
        
        [Header("References")]
        [SerializeField] private Rigidbody head;
        [SerializeField] private ConfigurableJoint leftHand;
        [SerializeField] private ConfigurableJoint rightHand;
        [SerializeField] private Rigidbody body;
        
        [Header("Speeds")]
        public float joystickMoveSpeed;    //  m / s
        public float joystickRotateSpeed;  //  deg / s


        [Header("Hand Physics")]
        public float handLinearSpringStrength;
        public float handLinearSpringDamper;
        public float handAngularSpringStrength;
        public float handAngularSpringDamper;

        protected override void Start()
        {
            vrInput = GetComponent<VRPlayerInput>();
        }

        private void OnValidate()
        {
            // Set joints
            var linearJointDrive = new JointDrive {positionSpring = handLinearSpringStrength, positionDamper = handLinearSpringDamper, maximumForce = Mathf.Infinity};
            var angularJointDrive = new JointDrive {positionSpring = handAngularSpringStrength, positionDamper = handAngularSpringDamper, maximumForce = Mathf.Infinity};
            ConfigurableJoint[] joints = {leftHand, rightHand}; 
            foreach (var joint in joints)
            {

                joint.xDrive = linearJointDrive;
                joint.yDrive = linearJointDrive;
                joint.zDrive = linearJointDrive;
                joint.angularXDrive = angularJointDrive;
                joint.angularYZDrive = angularJointDrive;
            }
        }

        private void FixedUpdate()
        {
            // Fetch input
            vrInput.Fetch();

            // Joystick & HMD movement
            var joystickMove = Quaternion.Euler(new Vector3(0, head.rotation.eulerAngles.y, 0)) * (joystickMoveSpeed * new Vector3(vrInput.move.x, 0, vrInput.move.y));
            var hmdMove      = body.rotation * new Vector3(vrInput.hmdDeltaPose.position.x, 0, vrInput.hmdDeltaPose.position.z);
            body.AddForce((joystickMove + hmdMove - new Vector3(body.velocity.x, 0, body.velocity.z)), ForceMode.VelocityChange);
            head.VelocityMove(new Vector3(0, vrInput.hmdPose.position.y, 0), true);
            
            // Joystick & HMD rotation
            body.MoveRotation(body.rotation * Quaternion.Euler(new Vector3(0, vrInput.rotate.x * joystickRotateSpeed * Time.fixedDeltaTime, 0)));
            head.transform.localRotation = vrInput.hmdPose.rotation;
            
            // Hands
            var jointRoot = new Vector3(vrInput.hmdPose.position.x, 0, vrInput.hmdPose.position.z);
            leftHand.targetPosition = vrInput.leftControllerPose.position - jointRoot;
            leftHand.targetRotation = vrInput.leftControllerPose.rotation;
            rightHand.targetPosition = vrInput.rightControllerPose.position - jointRoot;
            rightHand.targetRotation = vrInput.rightControllerPose.rotation;
        }

    }
}
