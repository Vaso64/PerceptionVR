using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Player;
using UnityEngine;
using System.Linq;

namespace PerceptionVR.Portal
{
    public class PortalRenderer : MonoBehaviour
    {
        // References
        private IPortal portal;
        
        [SerializeField] private Camera portalCamera;
        
        [SerializeField] private Renderer portalRend;

        private const int recurssionLimit = 3;

        private RenderTexture[] RTArray = new RenderTexture[recurssionLimit + 1];
        private Stack<int> RTArrayIndexStack = new Stack<int>(recurssionLimit);
        private bool RTArrayAlocated = false;

        private static List<PortalRenderer> allPortalRenderers = new List<PortalRenderer>();

        private void Awake()
        {
            // Register self and to events
            allPortalRenderers.Add(this);
            PlayerCamera.OnBeforePlayerCameraRender += OnBeforePlayerCameraRenderCallback;
            RenderingManagment.OnResolutionChange += AllocateRTArray;
        }

        private void Start()
        {
            // Get references
            portal ??= GetComponentInParent<IPortal>();
            portalCamera ??= GetComponentInChildren<Camera>();
        }

        private void AllocateRTArray(Vector2Int resolution)
        {
            // Free old RTs
            if (RTArrayAlocated)
                for (var i = 0; i <= recurssionLimit; i++)
                        RTArray[i].Release();

            // Allocate
            RTArrayAlocated = true;
            for (var i = 0; i <= recurssionLimit; i++)
                RTArray[i] = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.ARGB32);
        }


        private void OnBeforePlayerCameraRenderCallback(Camera playerCamera)
        {
            // Skip if not yet allocated
            if(!RTArrayAlocated)
                return;
            
            var visibleArea = new Rect(0, 0, RenderingManagment.currentResolution.x, RenderingManagment.currentResolution.y);
            var playerCameraFrustum = GeometryUtility.CalculateFrustumPlanes(playerCamera);
            var playerCameraPose = new Pose(playerCamera.transform.position, playerCamera.transform.rotation);
            if(IsVisibleFromCamera(playerCamera, playerCameraFrustum, visibleArea))
                RenderPortal(playerCameraPose, visibleArea, playerCamera.fieldOfView, 0);
        }


        public void OnAfterPortalRenderCallback()
        {
            // Set portal to previous texture after being rendered
            RTArrayIndexStack.Pop();
            portalRend.material.mainTexture = RTArrayIndexStack.Count > 0 ? RTArray[RTArrayIndexStack.Peek()] : null;

        }

        // Recursively renders portals
        public void RenderPortal(Pose fromPose, Rect visibleArea, float fov, int recursionDepth)
        {
            // Calculate visible area
            portalCamera.transform.SetPositionAndRotation(fromPose.position, fromPose.rotation);
            visibleArea = visibleArea.IntersectionWith(portalCamera.WorldToScreenBounds(portal.portalCollider.bounds));

            // Position camera & calculate frustum
            var pairPose = portal.PairPose(fromPose);
            portalCamera.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            portalCamera.fieldOfView = fov;
            var portalCameraFrustum = GeometryUtility.CalculateFrustumPlanes(portalCamera);
            

            // Get all visible portals
            var visiblePortalRenderers = new List<PortalRenderer>();
            if (recursionDepth < recurssionLimit)
                visiblePortalRenderers.AddRange(allPortalRenderers.Where(pr => pr.IsVisibleFromCamera(portalCamera, portalCameraFrustum, visibleArea)));

            // Render others
            visiblePortalRenderers.ForEach(pr => pr.RenderPortal(pairPose, visibleArea, fov, recursionDepth + 1));

            // Render self
            portalCamera.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            portalCamera.targetTexture = RTArray[recursionDepth];
            portalCamera.Render();

            // Notify others after render
            visiblePortalRenderers.ForEach(pr => pr.OnAfterPortalRenderCallback());

            // Set rendered texture and push it to stack
            RTArrayIndexStack.Push(recursionDepth);
            portalRend.material.mainTexture = portalCamera.targetTexture;
        }


        // Returns true if the portal is visible from the camera 
        public bool IsVisibleFromCamera(Camera camera, Plane[] cameraFrustum, Rect visibleArea)
        {
            // Dot product check (is camera behind the portal?)
            if(Vector3.Dot(transform.forward, transform.position - camera.transform.position) < 0)
                return false;

            // AABB frustum check (is portal in fov of the camera?)
            if (!GeometryUtility.TestPlanesAABB(cameraFrustum, portal.portalCollider.bounds))
                return false;

            // Overlap check (can portal be seen through another portal?)
            var portalRect = camera.WorldToScreenBounds(portal.portalCollider.bounds);
            visibleArea.IntersectionWith(portalRect);
            if(!visibleArea.Overlaps(portalRect))
                return false;

            return true;
        }
    }
}
