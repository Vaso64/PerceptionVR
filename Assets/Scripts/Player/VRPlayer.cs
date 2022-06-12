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
        private Transform LeftEye;

        [SerializeField]
        private Transform RightEye;

        [SerializeField]
        private Transform LeftHand;

        [SerializeField]
        private Transform RightHand;


        private PlayerInputAction.VRPlayerActions playerActions;

        private void Awake()
        {
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.VRPlayer;
        }

    }
}
