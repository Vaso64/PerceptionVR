using System;
using System.Collections.Generic;
using PerceptionVR.Portal;
using UnityEngine;

namespace PerceptionVR.Player
{
    public class PlayerCamera : MonoBehaviour, ITeleportableBehaviour
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
        

        public void OnCreateClone(GameObject clone, out IEnumerable<Type> preservedComponents)
        {
            preservedComponents = new[] { typeof(Camera), typeof(PlayerCamera) };
            clone.GetComponent<Camera>().enabled = false;
        } 
        
        public void TransferBehaviour(GameObject from, GameObject to)
        { 
            from.GetComponent<Camera>().enabled = false;
            to.GetComponent<Camera>().enabled = true;
        }


        private void Update()
        {
            /*
            Dbg.LogInfo($"({Time.time}) ((center)): {transform.position}");
            Dbg.LogInfo($"({Time.time}) ((center) PM):\n {this.playerCamera.projectionMatrix}");
            Dbg.LogInfo($"({Time.time}) ((center) VM):\n {this.playerCamera.worldToCameraMatrix}");
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
            Dbg.LogInfo($"({Time.time}) ({eye} SPM):\n {this.playerCamera.GetStereoProjectionMatrix(eye)}");
            Dbg.LogInfo($"({Time.time}) ({eye} SVM):\n {this.playerCamera.GetStereoViewMatrix(eye)}");
            Dbg.LogInfo($"({Time.time}) ({eye} PM):\n {this.playerCamera.projectionMatrix}");
            Dbg.LogInfo($"({Time.time}) ({eye} VM):\n {this.playerCamera.worldToCameraMatrix}");
            */

            OnBeforePlayerCameraRender?.Invoke(this.playerCamera);
        } 
    }
}