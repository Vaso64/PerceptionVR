using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PerceptionVR.Extensions;
using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Player
{
    public class PlayerCamera: MonoBehaviourBase, ITeleportableBehaviour
    {
        public Rect viewportRect = new (0, 0, 1, 1);
        public Matrix4x4 projectionMatrix;
        public bool pmDirection;
        public bool scissor;
        public bool resetProjectionMatrix;
        
        private Camera playerCamera;
        
        private Matrix4x4 currentViewMatrix;
        private Matrix4x4 currentProjectionMatrix;
        
        
        [SerializeField] private PortalRenderGroup currentRenderGroup;
        
        
        private void Start()
        {
            playerCamera = GetComponent<Camera>();
            playerCamera.nearClipPlane = 0.0001f;
            // Portal group belonging to the closest portal
            currentRenderGroup = FindObjectsOfType<PortalRenderer>().MinBy(renderer => Vector3.Distance(transform.position, renderer.transform.position)).Min().renderGroup;
            GetComponentInParent<TeleportableObject>().OnTeleport += teleportData => currentRenderGroup = teleportData.outPortal.GetComponent<PortalRenderer>().renderGroup;
        }
        

        public void OnCreateClone(CloneData cloneData, out IEnumerable<Type> preservedComponents)
        {
            preservedComponents = new[] { typeof(Camera), typeof(PlayerCamera) };
            cloneData.clone.GetComponent<Camera>().enabled = false;
        }

        public void OnEnterPortal(CloneData cloneData)
        {
            cloneData.clone.GetComponent<Camera>().enabled    = true;
            cloneData.original.GetComponent<Camera>().enabled = false;
        }

        public void OnExitPortal(CloneData cloneData)
        {
            cloneData.clone.GetComponent<Camera>().enabled    = false;
            cloneData.original.GetComponent<Camera>().enabled = true;
        }

        public void OnTeleport(CloneData cloneData)
        {
            var cloneCamera = cloneData.clone.GetComponent<Camera>();
            var originalCamera = cloneData.original.GetComponent<Camera>();
            (cloneCamera.enabled, originalCamera.enabled) = (originalCamera.enabled, cloneCamera.enabled);
        }


        private void Update()
        {
            //playerCamera.ResetProjectionMatrix();
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
            
            // Scissor rect
            //playerCamera.ResetProjectionMatrix();

            if (scissor)
            {
                playerCamera.SetScissorRect(viewportRect);
                scissor = false;
            }
            
            if (resetProjectionMatrix)
            {
                playerCamera.ResetProjectionMatrix();
                resetProjectionMatrix = false;
            }
            
            //playerCamera.ResetProjectionMatrix();
            //playerCamera.rect = new Rect(0, 0, 1, 1);   
            //playerCamera.SetScissorRect(viewportRect);
            
            
                
            if(pmDirection) playerCamera.projectionMatrix = projectionMatrix;
            else            projectionMatrix = playerCamera.projectionMatrix;

            // Render portals
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
            foreach (var portalRenderer in currentRenderGroup)
                portalRenderer.OnBeforePlayerCameraRenderCallback(playerCamera, frustumPlanes);

            //OnBeforePlayerCameraRender?.Invoke(this.playerCamera, GeometryUtility.CalculateFrustumPlanes(this.playerCamera));
        } 
    }
}