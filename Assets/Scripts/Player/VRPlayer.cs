using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PerceptionVR.Player
{
    public class VRPlayer : PlayerBase
    {
        // XR References
        [SerializeField]
        private Transform head;

        [SerializeField]
        private Transform leftHand;

        [SerializeField]
        private Transform rightHand;


        private PlayerInputAction.VRPlayerActions playerActions;

        private void Awake()
        {
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.VRPlayer;
        }

        private void Update()
        {
            // Left Hand
            leftHand.localPosition = playerActions.LeftHandPosition.ReadValue<Vector3>();
            leftHand.localRotation = playerActions.LeftHandRotation.ReadValue<Quaternion>();

            // Right Hand
            rightHand.localPosition = playerActions.RightHandPosition.ReadValue<Vector3>();
            rightHand.localRotation = playerActions.RightHandRotation.ReadValue<Quaternion>();

            // Head
            head.localPosition = playerActions.HeadPosition.ReadValue<Vector3>();
            head.localRotation = playerActions.HeadRotation.ReadValue<Quaternion>();
        }

    }
}
