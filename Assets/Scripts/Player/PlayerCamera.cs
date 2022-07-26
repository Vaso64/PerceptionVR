using System;
using UnityEngine;

namespace PerceptionVR.Player
{
    [RequireComponent(typeof (Camera))]
    public class PlayerCamera : MonoBehaviour
    {
        public static event Camera.CameraCallback OnBeforePlayerCameraRender;

        private Camera playerCamera;
        
        private Matrix4x4 currentViewMatrix;
        private Matrix4x4 currentProjectionMatrix;

        private void Start()
        {
            playerCamera = GetComponent<Camera>();
            playerCamera.nearClipPlane = 0.00001f;
        }

        private void Update()
        {
            /*
            Debug.Log($"({Time.time}) ((center)): {transform.position}");
            Debug.Log($"({Time.time}) ((center) PM):\n {this.playerCamera.projectionMatrix}");
            Debug.Log($"({Time.time}) ((center) VM):\n {this.playerCamera.worldToCameraMatrix}");
            */
            this.currentProjectionMatrix = this.playerCamera.projectionMatrix;
            this.currentViewMatrix = this.playerCamera.worldToCameraMatrix;
        }

        private void OnPreRender()
        {
            if(playerCamera.stereoEnabled)
            {
                playerCamera.projectionMatrix = this.currentProjectionMatrix;
                playerCamera.worldToCameraMatrix = this.currentViewMatrix;
            }

            /*var eye = playerCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;
            Debug.Log($"({Time.time}) ({eye} SPM):\n {this.playerCamera.GetStereoProjectionMatrix(eye)}");
            Debug.Log($"({Time.time}) ({eye} SVM):\n {this.playerCamera.GetStereoViewMatrix(eye)}");
            Debug.Log($"({Time.time}) ({eye} PM):\n {this.playerCamera.projectionMatrix}");
            Debug.Log($"({Time.time}) ({eye} VM):\n {this.playerCamera.worldToCameraMatrix}");
            */

            OnBeforePlayerCameraRender?.Invoke(this.playerCamera);
        } 
    }
}