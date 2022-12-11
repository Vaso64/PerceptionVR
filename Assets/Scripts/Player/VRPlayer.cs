using PerceptionVR.Debug;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Portal;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof(VRPlayerInput))]
    public class VRPlayer : PlayerBase
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

        [Header("Movement")]
        public float joystickMoveSpeed;    //  m / s
        public float joystickRotateSpeed;  //  deg / s
        public float jumpForce;            

        [Header("Hand Physics")]
        public float handLinearSpringStrength;
        public float handLinearSpringDamper;
        public float handAngularSpringStrength;
        public float handAngularSpringDamper;

        protected void Start()
        {
            vrInput = GetComponent<VRPlayerInput>();
            
            vrInput.OnJump += Jump;
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
            var headY = Quaternion.Euler((Quaternion.Inverse(gravityDirection) * headPivot.rotation).eulerAngles._0y0());
            var joystickMove = gravityDirection * (headY * vrInput.move.x0y() * joystickMoveSpeed * Time.fixedDeltaTime);
            var hmdMove = body.rotation * vrInput.hmdDeltaPose.position.x0z();
            
            // Joystick & HMD position
            body.MovePosition(body.position + (joystickMove + hmdMove));
            head.targetPosition = vrInput.hmdPose.position._0y0();
            
            // Joystick & HMD rotation
            body.MoveRotation(body.rotation * Quaternion.Euler(joystickRotateSpeed * Time.fixedDeltaTime * vrInput.rotate._0x0()));
            headPivot.transform.localRotation = vrInput.hmdPose.rotation;
            
            // Eyes
            if(!customIpd) 
                ipd = Vector3.Distance(vrInput.leftEyePose.position, vrInput.rightEyePose.position);
            leftEye.transform.localPosition = new Vector3(-ipd / 2, 0, 0);
            rightEye.transform.localPosition = new Vector3(ipd / 2, 0, 0);

            // Hands
            var jointRoot = vrInput.hmdPose.position.x0z();
            leftHand.targetPosition = vrInput.leftControllerPose.position - jointRoot;
            if(!vrInput.leftControllerPose.rotation.IsNaN())
                leftHand.targetRotation = vrInput.leftControllerPose.rotation;
            rightHand.targetPosition = vrInput.rightControllerPose.position - jointRoot;
            if(!vrInput.rightControllerPose.rotation.IsNaN())
                rightHand.targetRotation = vrInput.rightControllerPose.rotation;
        }
        
        private void Jump()
        {
            body.AddForce(gravityDirection * Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }
}
