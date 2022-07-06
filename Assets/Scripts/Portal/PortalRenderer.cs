using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Player;
using UnityEngine;
using System.Linq;

namespace PerceptionVR.Portal
{
    [RequireComponent(typeof(PortalBase))]
    public class PortalRenderer : MonoBehaviour
    {
        private const int recurssionLimit = 3;

        private RenderTexture[] RTArray = new RenderTexture[recurssionLimit + 1];
        private Stack<int> RTArrayIndexStack = new Stack<int>(recurssionLimit);
        private bool RTArrayAlocated = false;

        [SerializeField]
        private Renderer portalRend;

        public Collider portalCollider;

        private PortalRenderer[] allPortalRenderers;

        private PortalBase portalBase;
        
        private Camera portalCamera;

        
        private void Start()
        {
            // Get references
            portalBase = GetComponent<PortalBase>();
            portalCamera = GetComponentInChildren<Camera>();
            portalCollider = GetComponentInChildren<Collider>();
            allPortalRenderers = FindObjectsOfType<PortalRenderer>();

            // Register to events
            PlayerCamera.OnBeforePlayerCameraRender += OnBeforePlayerCameraRenderCallback;
            RenderingManagment.OnResolutionChange += AllocateRTArray;
        }

        private void AllocateRTArray(Vector2Int resolution)
        {
            // Free old RTs
            if (RTArrayAlocated)
                for (int i = 0; i <= recurssionLimit; i++)
                        RTArray[i].Release();

            // Allocate
            RTArrayAlocated = true;
            for (int i = 0; i <= recurssionLimit; i++)
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

        // Recursivly renders portals
        public void RenderPortal(Pose fromPose, Rect visibleArea, float fov, int recurssionDepth)
        {
            // Calculate visible area
            portalCamera.transform.SetPositionAndRotation(fromPose.position, fromPose.rotation);
            visibleArea = visibleArea.IntersectionWith(portalCamera.WorldToScreenBounds(portalCollider.bounds));

            // Position camera & calculate frustum
            var pairPose = portalBase.CalculatePairPose(fromPose);
            portalCamera.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            portalCamera.fieldOfView = fov;
            var portalCameraFrustum = GeometryUtility.CalculateFrustumPlanes(portalCamera);
            

            // Get all visible portals
            var visiblePortalRenderers = new List<PortalRenderer>();
            if (recurssionDepth < recurssionLimit)
                visiblePortalRenderers.AddRange(allPortalRenderers.Where(pr => pr.IsVisibleFromCamera(portalCamera, portalCameraFrustum, visibleArea)));

            // Render others
            visiblePortalRenderers.ForEach(pr => pr.RenderPortal(pairPose, visibleArea, fov, recurssionDepth + 1));

            // Render self
            portalCamera.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            portalCamera.targetTexture = RTArray[recurssionDepth];
            portalCamera.Render();

            // Notify others after render
            visiblePortalRenderers.ForEach(pr => pr.OnAfterPortalRenderCallback());

            // Set rendered texture and push it to stack
            RTArrayIndexStack.Push(recurssionDepth);
            portalRend.material.mainTexture = portalCamera.targetTexture;
        }


        // Returns true if the portal is visible from the camera 
        public bool IsVisibleFromCamera(Camera camera, Plane[] cameraFrustum, Rect visibleArea)
        {
            // Dot product check (is camera behind the portal?)
            if(Vector3.Dot(transform.forward, transform.position - camera.transform.position) < 0)
                return false;

            // AABB frustum check (is portal in fov of the camera?)
            if (!GeometryUtility.TestPlanesAABB(cameraFrustum, portalCollider.bounds))
                return false;

            // Overlap check (can portal be seen through another portal?)
            var portalRect = camera.WorldToScreenBounds(portalCollider.bounds);
            visibleArea.IntersectionWith(portalRect);
            if(!visibleArea.Overlaps(portalRect))
                return false;

            return true;
        }
    }
}
