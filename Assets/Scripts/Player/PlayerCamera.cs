using System;
using System.Collections.Generic;
using System.Linq;
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
        private (PortalRenderer portalRenderer, Rect visibleArea)[] visiblePortals;
        
        
        private void Start()
        {
            cameraComponent = GetComponent<Camera>();
            cameraComponent.nearClipPlane = 0.0001f;
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
            visiblePortals = currentRenderGroup.GetVisible(cameraComponent).ToArray();
            foreach (var visiblePortal in visiblePortals)
                visiblePortal.portalRenderer.StartRenderPortalChain(cameraComponent, visiblePortal.visibleArea);
        }

        private void OnPostRender()
        {
            foreach (var visiblePortal in visiblePortals)
                visiblePortal.portalRenderer.OnAfterParentRender();
        }
    }
}