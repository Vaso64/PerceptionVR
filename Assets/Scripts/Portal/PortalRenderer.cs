using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Player;
using UnityEngine;
using System.Linq;

namespace PerceptionVR.Portal
{
    [RequireComponent(typeof(PortalBase))]
    public class PortalRenderer : MonoBehaviour
    {
        private const int recurssionLimit = 3;

        private RenderTexture[] RTArray;
        private Stack<int> RTArrayIndexStack;

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

            // Create portal renderer states array
            RTArray = new RenderTexture[recurssionLimit + 1];
            RTArrayIndexStack = new Stack<int>(recurssionLimit);
            for (int i = 0; i <= recurssionLimit; i++)
                RTArray[i] = new RenderTexture(Screen.width, Screen.height, 24);

            // Register to events
            PlayerCamera.OnBeforePlayerCameraRender += OnBeforePlayerCameraRenderCallback;
        }


        private void OnBeforePlayerCameraRenderCallback(Camera playerCamera)
        {
            var visibleArea = new Rect(0, 0, Screen.width, Screen.height);
            var playerCameraFrustum = GeometryUtility.CalculateFrustumPlanes(playerCamera);
            var playerCameraPose = new Pose(playerCamera.transform.position, playerCamera.transform.rotation);
            if(IsVisibleFromCamera(playerCamera, playerCameraFrustum, visibleArea))
                RenderPortal(playerCameraPose, visibleArea, 0);
        }


        public void OnAfterPortalRenderCallback()
        {
            // Set portal to previous texture after being rendered
            RTArrayIndexStack.Pop();
            portalRend.material.mainTexture = RTArrayIndexStack.Count > 0 ? RTArray[RTArrayIndexStack.Peek()] : null;

        }

        // Recursivly renders portals
        public void RenderPortal(Pose fromPose, Rect visibleArea, int recurssionDepth)
        {
            // Calculate visible area
            portalCamera.transform.SetPositionAndRotation(fromPose.position, fromPose.rotation);
            visibleArea = visibleArea.IntersectionWith(portalCamera.WorldToScreenBounds(portalCollider.bounds));

            // Position camera & calculate frustum
            var pairPose = portalBase.CalculatePairPose(fromPose);
            portalCamera.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            var portalCameraFrustum = GeometryUtility.CalculateFrustumPlanes(portalCamera);
            

            // Get all visible portals
            var visiblePortalRenderers = new List<PortalRenderer>();
            if (recurssionDepth < recurssionLimit)
                visiblePortalRenderers.AddRange(allPortalRenderers.Where(pr => pr.IsVisibleFromCamera(portalCamera, portalCameraFrustum, visibleArea)));

            // Render others
            visiblePortalRenderers.ForEach(pr => pr.RenderPortal(pairPose, visibleArea, recurssionDepth + 1));

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
