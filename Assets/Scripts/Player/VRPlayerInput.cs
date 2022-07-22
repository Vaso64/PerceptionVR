using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PerceptionVR.Player
{
    public class VRPlayerInput : MonoBehaviour
    {
        private PlayerInputAction.VRPlayerActions playerActions;

        [Header("Debugging")]
        [SerializeField] private bool debugMode = false;
        [Space(8)]
        [SerializeField] private Transform debugHmd;
        [SerializeField] private Transform debugLeftController;
        [SerializeField] private Transform debugRightController;
        [SerializeField] private Vector2 debugMove;
        [SerializeField] private Vector2 debugRotate;
    
        // Input readings
        [HideInInspector] public Pose hmdPose;
        [HideInInspector] public Pose hmdDeltaPose;
        [HideInInspector] public Pose leftEyePose;
        [HideInInspector] public Pose leftEyeDeltaPose;
        [HideInInspector] public Pose rightEyePose;
        [HideInInspector] public Pose rightEyeDeltaPose;
        [HideInInspector] public Pose leftControllerPose;
        [HideInInspector] public Pose leftControllerDeltaPose;
        [HideInInspector] public Pose rightControllerPose;
        [HideInInspector] public Pose rightControllerDeltaPose;
        [HideInInspector] public Vector2 move;
        [HideInInspector] public Vector2 rotate;
        public Action rightControllerGrabbed;
        public Action rightControllerReleased;
        public Action leftControllerGrabbed;
        public Action leftControllerReleased;
    
        private void Awake()
        {
            var playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            this.playerActions = playerInputAction.VRPlayer;
            FixedUpdate();
        }

        private void Start()
        {
            playerActions.RightControllerGrab.started += ctx => rightControllerGrabbed?.Invoke();
            playerActions.RightControllerGrab.canceled += ctx => rightControllerReleased?.Invoke();
            playerActions.LeftControllerGrab.started += ctx => leftControllerGrabbed?.Invoke();
            playerActions.LeftControllerGrab.canceled += ctx => leftControllerReleased?.Invoke();
        }

        public void FixedUpdate()
        {
            if(debugMode) FetchDebugInput();
            else          FetchRealInput();
        }

        private void FetchRealInput()
        {
            Vector3 tempPosition; 
            Quaternion tempRotation;

            // HMD position
            tempPosition = playerActions.HMDPosition.ReadValue<Vector3>();
            hmdDeltaPose.position = tempPosition - hmdPose.position;
            hmdPose.position = tempPosition;
        
            // HMD rotation
            tempRotation = playerActions.HMDRotation.ReadValue<Quaternion>();
            hmdDeltaPose.rotation = tempRotation * Quaternion.Inverse(hmdPose.rotation);
            hmdPose.rotation = tempRotation;
            
            // Left eye position
            tempPosition = playerActions.LeftEyePosition.ReadValue<Vector3>();
            leftEyeDeltaPose.position = tempPosition - leftEyePose.position;
            leftEyePose.position = tempPosition;
            
            // Left eye rotation
            tempRotation = playerActions.LeftEyeRotation.ReadValue<Quaternion>();
            leftEyeDeltaPose.rotation = tempRotation * Quaternion.Inverse(leftEyePose.rotation);
            leftEyePose.rotation = tempRotation;
            
            // Right eye position
            tempPosition = playerActions.RightEyePosition.ReadValue<Vector3>();
            rightEyeDeltaPose.position = tempPosition - rightEyePose.position;
            rightEyePose.position = tempPosition;
            
            // Right eye rotation
            tempRotation = playerActions.RightEyeRotation.ReadValue<Quaternion>();
            rightEyeDeltaPose.rotation = tempRotation * Quaternion.Inverse(rightEyePose.rotation);
            rightEyePose.rotation = tempRotation;

            // Left controller position
            tempPosition = playerActions.LeftControllerPosition.ReadValue<Vector3>();
            leftControllerDeltaPose.position = tempPosition - leftControllerPose.position;
            leftControllerPose.position = tempPosition;
        
            // Left controller rotation
            tempRotation = playerActions.LeftControllerRotation.ReadValue<Quaternion>();
            leftControllerDeltaPose.rotation = tempRotation * Quaternion.Inverse(leftControllerPose.rotation);
            leftControllerPose.rotation = tempRotation;
        
            // Right controller position
            tempPosition = playerActions.RightControllerPosition.ReadValue<Vector3>();
            rightControllerDeltaPose.position = tempPosition - rightControllerPose.position;
            rightControllerPose.position = tempPosition;
        
            // Right controller rotation
            tempRotation = playerActions.RightControllerRotation.ReadValue<Quaternion>();
            rightControllerDeltaPose.rotation = tempRotation * Quaternion.Inverse(rightControllerPose.rotation);
            rightControllerPose.rotation = tempRotation;
        
            // Joysticks
            move = playerActions.Move.ReadValue<Vector2>();
            rotate = playerActions.Rotate.ReadValue<Vector2>();
        
            /*
            // Left grab / release
            if (playerActions.LeftControllerGrab.WasPressedThisFrame())
                leftControllerGrabbed?.Invoke();
            if(playerActions.LeftControllerGrab.WasReleasedThisFrame()) 
                leftControllerReleased?.Invoke();
            
            // Right grab / release
            if (playerActions.RightControllerGrab.WasPressedThisFrame())
                rightControllerGrabbed?.Invoke();
            if(playerActions.RightControllerGrab.WasReleasedThisFrame()) 
                rightControllerReleased?.Invoke();
            */
        }

        private void FetchDebugInput()
        {
            // HMD position
            hmdDeltaPose.position = debugHmd.localPosition - hmdPose.position;
            hmdPose.position = debugHmd.localPosition;
        
            // HMD rotation
            hmdDeltaPose.rotation = debugHmd.localRotation * Quaternion.Inverse(hmdPose.rotation);
            hmdPose.rotation = debugHmd.localRotation;
        
            // Left controller position
            leftControllerDeltaPose.position = debugLeftController.localPosition - leftControllerPose.position;
            leftControllerPose.position = debugLeftController.localPosition;
        
            // Left controller rotation
            leftControllerDeltaPose.rotation = debugLeftController.localRotation * Quaternion.Inverse(leftControllerPose.rotation);
            leftControllerPose.rotation = debugLeftController.localRotation;
        
            // Right controller position
            rightControllerDeltaPose.position = debugRightController.localPosition - rightControllerPose.position;
            rightControllerPose.position =  debugRightController.localPosition;
        
            // Right controller rotation
            rightControllerDeltaPose.rotation = debugRightController.localRotation * Quaternion.Inverse(rightControllerPose.rotation);
            rightControllerPose.rotation = debugRightController.localRotation;
        
            // Joysticks
            move.x =   Mathf.Clamp(debugMove.x,   -1, 1);
            move.y =   Mathf.Clamp(debugMove.y,   -1, 1);
            rotate.x = Mathf.Clamp(debugRotate.x, -1, 1);
            rotate.y = Mathf.Clamp(debugRotate.y, -1, 1);
        }
    }
}