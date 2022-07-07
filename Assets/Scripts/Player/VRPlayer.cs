using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PerceptionVR.Extensions;
using UnityEngine.InputSystem;

namespace PerceptionVR.Player
{
    public class VRPlayer : PlayerBase
    {
        // XR References
        [SerializeField]
        private Transform head;

        [SerializeField]
        private Transform leftController;
        
        [SerializeField]
        private  Transform rightController;

        [SerializeField]
        private Rigidbody leftHand;

        [SerializeField]
        private Rigidbody rightHand;

        public float moveSpeed;
        public float rotateSpeed;

        public float controllerToHandIntensity;


        private PlayerInputAction.VRPlayerActions playerActions;

        private void Awake()
        {
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.VRPlayer;
        }

        private void FixedUpdate()
        {
            // Left 
            leftController.localPosition = playerActions.LeftHandPosition.ReadValue<Vector3>();
            leftController.localRotation = playerActions.LeftHandRotation.ReadValue<Quaternion>();
            leftHand.velocity = (leftController.position - leftHand.position) * controllerToHandIntensity;
            leftHand.angularVelocity = (leftController.rotation * Quaternion.Inverse(leftHand.transform.rotation)).GetClampedEulerAngles();

            // Right
            rightController.localPosition = playerActions.RightHandPosition.ReadValue<Vector3>();
            rightController.localRotation = playerActions.RightHandRotation.ReadValue<Quaternion>();
            rightHand.velocity = (rightController.position - rightHand.position) * controllerToHandIntensity;
            rightHand.angularVelocity = (rightController.rotation * Quaternion.Inverse(rightHand.transform.rotation)).GetClampedEulerAngles();

            // Head
            head.localPosition = playerActions.HeadPosition.ReadValue<Vector3>();
            head.localRotation = playerActions.HeadRotation.ReadValue<Quaternion>();

            // Body
            var move = playerActions.Move.ReadValue<Vector2>();
            var rotate = playerActions.Rotate.ReadValue<Vector2>();
            transform.position += move.y * moveSpeed * Time.fixedDeltaTime * Vector3.Scale(head.forward, new Vector3(1,0,1)).normalized;
            transform.position += move.x * moveSpeed * Time.fixedDeltaTime * Vector3.Scale(head.right, new Vector3(1,0,1)).normalized;
            transform.RotateAround(head.position, Vector3.up, rotate.x * rotateSpeed * Time.fixedDeltaTime);
        }

    }
}
