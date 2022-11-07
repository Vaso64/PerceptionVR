using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Portal;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(VRPlayerInput))]
    public class VRPlayer : PlayerBase, ITeleportable
    {
        private VRPlayerInput vrInput;

        [SerializeField] private bool customIpd = false;
        [SerializeField] [Range(0, 1f)] private float ipd;
        
        [Header("References")]
        [SerializeField] private Transform leftEye;
        [SerializeField] private Transform rightEye;
        [SerializeField] private ConfigurableJoint head;
        [SerializeField] private Transform headPivot;
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
            ConfigurableJoint[] joints = {leftHand, rightHand, head}; 
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
            // Joystick & HMD movement
            var joystickMove = Quaternion.Euler(new Vector3(0, headPivot.transform.rotation.eulerAngles.y, 0)) * (joystickMoveSpeed * new Vector3(vrInput.move.x, 0, vrInput.move.y));
            var hmdMove      = body.rotation * new Vector3(vrInput.hmdDeltaPose.position.x, 0, vrInput.hmdDeltaPose.position.z) / Time.fixedDeltaTime;
            body.AddForce(joystickMove + hmdMove - new Vector3(body.velocity.x, 0, body.velocity.z), ForceMode.VelocityChange);
            head.targetPosition = new Vector3(0, vrInput.hmdPose.position.y, 0);
            
            // Joystick & HMD rotation
            body.MoveRotation(body.rotation * Quaternion.Euler(new Vector3(0, vrInput.rotate.x * joystickRotateSpeed * Time.fixedDeltaTime, 0)));
            headPivot.transform.localRotation = vrInput.hmdPose.rotation;
            
            // Eyes
            if(!customIpd) 
                ipd = Vector3.Distance(vrInput.leftEyePose.position, vrInput.rightEyePose.position);
            leftEye.transform.localPosition = new Vector3(-ipd / 2, 0, 0);
            rightEye.transform.localPosition = new Vector3(ipd / 2, 0, 0);

            // Hands
            var jointRoot = new Vector3(vrInput.hmdPose.position.x, 0, vrInput.hmdPose.position.z);
            leftHand.targetPosition = vrInput.leftControllerPose.position - jointRoot;
            if(!vrInput.leftControllerPose.rotation.IsNaN())
                leftHand.targetRotation = vrInput.leftControllerPose.rotation;
            rightHand.targetPosition = vrInput.rightControllerPose.position - jointRoot;
            if(!vrInput.rightControllerPose.rotation.IsNaN())
                rightHand.targetRotation = vrInput.rightControllerPose.rotation;
        }

    }
}
