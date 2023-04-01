using System;
using System.Collections.Generic;
using PerceptionVR.Debug;
using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Player
{
    public class PlayerCamera : MonoBehaviour, ITeleportableBehaviour
    {
        public static event Action<Camera, Plane[]> OnBeforePlayerCameraRender;
        
        private Camera playerCamera;
        
        private Matrix4x4 currentViewMatrix;
        private Matrix4x4 currentProjectionMatrix;

        private void Start()
        {
            playerCamera = GetComponent<Camera>();
            playerCamera.nearClipPlane = 0.0001f;
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
            playerCamera.ResetProjectionMatrix();
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

            OnBeforePlayerCameraRender?.Invoke(this.playerCamera, GeometryUtility.CalculateFrustumPlanes(this.playerCamera));
        } 
    }
}