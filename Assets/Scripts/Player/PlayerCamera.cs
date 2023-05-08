using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using PerceptionVR.Portals;
using UnityEngine;

namespace PerceptionVR.Player
{
    public class PlayerCamera : MonoBehaviourBase, ITeleportableBehaviour
    {
        private Camera cameraComponent;
        
        private Matrix4x4 currentViewMatrix;
        private Matrix4x4 currentProjectionMatrix;
        
        
        [SerializeField] private PortalRenderGroup currentRenderGroup;
        
        
        private void Start()
        {
            cameraComponent = GetComponent<Camera>();
            cameraComponent.nearClipPlane = 0.0001f;
            // Portal group belonging to the closest portal
            //currentRenderGroup = FindObjectsOfType<PortalRenderer>().MinBy(renderer => Vector3.Distance(transform.position, renderer.transform.position)).Min().renderGroup;
            //if(this.TryGetComponentInParent(out TeleportableObject teleportableObject))
            //    teleportableObject.OnTeleport += teleportData => currentRenderGroup = teleportData.outPortal.GetComponent<PortalRenderer>().renderGroup;
        }
        

        public void OnCreateClone(CloneData cloneData, out IEnumerable<Type> preservedComponents)
        {
            preservedComponents = new[] { typeof(Camera), typeof(PlayerCamera) };
            cloneData.clone.GetComponentInChildren<Camera>().enabled = false;;
            cloneData.clone.GetComponentInChildren<PlayerCamera>().currentRenderGroup = cloneData.outPortal.GetComponent<PortalRenderer>().renderGroup;
            
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
            var cloneCamera = cloneData.clone.GetComponent<PlayerCamera>();
            var originalCamera = cloneData.original.GetComponent<PlayerCamera>();
            (cloneCamera.cameraComponent.enabled, originalCamera.cameraComponent.enabled) = (originalCamera.cameraComponent.enabled, cloneCamera.cameraComponent.enabled);
            (cloneCamera.currentRenderGroup, originalCamera.currentRenderGroup) = (originalCamera.currentRenderGroup, cloneCamera.currentRenderGroup);
        }


        private void Update()
        {
            //playerCamera.ResetProjectionMatrix();
            this.currentProjectionMatrix = this.cameraComponent.projectionMatrix;
            this.currentViewMatrix = this.cameraComponent.worldToCameraMatrix;
        }

        private void OnPreRender()
        {
            if(cameraComponent.stereoEnabled)
            {
                cameraComponent.projectionMatrix = this.currentProjectionMatrix;
                cameraComponent.worldToCameraMatrix = this.currentViewMatrix;
            }

            // Render portals
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cameraComponent);
            foreach (var portalRenderer in currentRenderGroup)
                portalRenderer.OnBeforePlayerCameraRenderCallback(cameraComponent, frustumPlanes);
        } 
    }
}