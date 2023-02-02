using System;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Common;
using PerceptionVR.Player;
using UnityEngine;
using System.Linq;
using PerceptionVR.Debug;

namespace PerceptionVR.Portal
{
    [RequireComponent(typeof(Renderer))]
    public class PortalRenderer : MonoBehaviour
    {
        // References
        [SerializeField] private Camera portalCamera;
        
        private Portal portal;
        private Renderer portalRend;

        private const int recurssionLimit = 3;

        private Dictionary<DisplayMode, RenderTexture[]> RTArrays = new () {
            { DisplayMode.VR, new RenderTexture[recurssionLimit + 1] },
            { DisplayMode.Desktop, new RenderTexture[recurssionLimit + 1] }
        };
        private Dictionary<DisplayMode, Stack<int>> RTArraysIndexStack = new () {
            { DisplayMode.VR, new Stack<int>(recurssionLimit) },
            { DisplayMode.Desktop, new Stack<int>(recurssionLimit) }
        };

        private static List<PortalRenderer> allPortalRenderers = new();

        private void Awake()
        {
            RenderingManagment.OnResolutionChange += AllocateRTArray;
            
            // Get references
            portal = GetComponentInParent<Portal>();
            portalRend = GetComponent<Renderer>();
            portalCamera ??= GetComponentInChildren<Camera>();

            portal.OnPortalPairSet += _ =>
            {
                allPortalRenderers.Add(this);
                PlayerCamera.OnBeforePlayerCameraRender += OnBeforePlayerCameraRenderCallback;
            };

            portal.OnPortalPairUnset += () => 
            {
                allPortalRenderers.Remove(this);
                PlayerCamera.OnBeforePlayerCameraRender -= OnBeforePlayerCameraRenderCallback;
            };
        }

        private void AllocateRTArray(DisplayMode displayMode, Vector2Int resolution)
        {
            // Free old RTs
            for (var i = 0; i <= recurssionLimit; i++)
                        RTArrays[displayMode][i]?.Release();

            // Allocate
            for (var i = 0; i <= recurssionLimit; i++)
                RTArrays[displayMode][i] = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.Default);
        }


        private void OnBeforePlayerCameraRenderCallback(Camera playerCamera)
        {
            var displayMode = playerCamera.stereoTargetEye.ToGameDisplayMode();
            var visibleArea = new Rect(0, 0, RenderingManagment.CurrentResolutions[displayMode].x, RenderingManagment.CurrentResolutions[displayMode].y);
            var playerCameraFrustum = GeometryUtility.CalculateFrustumPlanes(playerCamera);
            var playerCameraPose = new Pose(playerCamera.transform.position, playerCamera.transform.rotation);
            if(IsVisibleFromCamera(playerCamera, playerCameraFrustum, visibleArea))
                RenderPortal(playerCameraPose, visibleArea, playerCamera.fieldOfView, 0, displayMode);
        }


        public void OnAfterPortalRenderCallback(DisplayMode displayMode)
        {
            // Set portal to previous texture after being rendered
            RTArraysIndexStack[displayMode].Pop();
            portalRend.material.mainTexture = RTArraysIndexStack[displayMode].Count > 0 ? RTArrays[displayMode][RTArraysIndexStack[displayMode].Peek()] : null;
        }

        // Recursively renders portals
        public void RenderPortal(Pose fromPose, Rect visibleArea, float fov, int recursionDepth, DisplayMode displayMode)
        {
            // Calculate visible area
            portalCamera.transform.SetPose(fromPose);
            portalCamera.fieldOfView = fov;
            portalCamera.targetTexture = RTArrays[displayMode][recursionDepth]; // Set's camera resolution
            visibleArea = visibleArea.IntersectionWith(portalCamera.WorldToScreenBounds(portal.portalCollider.bounds));

            // Position camera
            var pairPose = portal.PairPose(fromPose);
            portalCamera.transform.SetPose(pairPose);

            // Get all visible portals
            var visiblePortalRenderers = new List<PortalRenderer>();
            if (recursionDepth < recurssionLimit)
            {
                var frustum = GeometryUtility.CalculateFrustumPlanes(portalCamera);
                visiblePortalRenderers = allPortalRenderers.Where(p => p.IsVisibleFromCamera(portalCamera, frustum, visibleArea)).ToList();
            }

            
            // Render others
            visiblePortalRenderers.ForEach(pr => pr.RenderPortal(pairPose, visibleArea, fov, recursionDepth + 1, displayMode));

            // Render self
            portalCamera.transform.SetPose(pairPose);
            portalCamera.targetTexture = RTArrays[displayMode][recursionDepth];
            portalCamera.projectionMatrix = ClipNearPlane(portalCamera, portal.portalPair);
            portalCamera.Render();
            portalCamera.ResetProjectionMatrix();

            // Notify others after render
            visiblePortalRenderers.ForEach(pr => pr.OnAfterPortalRenderCallback(displayMode));

            // Set rendered texture and push it to stack
            RTArraysIndexStack[displayMode].Push(recursionDepth);
            portalRend.material.mainTexture = portalCamera.targetTexture;
        }


        // Returns true if the portal is visible from the camera 
        public bool IsVisibleFromCamera(Camera camera, Plane[] cameraFrustum, Rect visibleArea)
        {
            // Dot product check (is camera behind the portal?)
            if (Vector3.Dot(transform.forward, transform.position - camera.transform.position) < 0)
                return false;
            
            // AABB frustum check (is portal in fov of the camera?)
            if (!GeometryUtility.TestPlanesAABB(cameraFrustum, portal.portalCollider.bounds))
                return false;
            
            // Overlap check (can portal be seen through another portal?)
            var portalRect = camera.WorldToScreenBounds(portal.portalCollider.bounds);
            if(!visibleArea.IntersectionWith(portalRect).Overlaps(portalRect))
                return false;

            return true;
        }

        private static Matrix4x4 ClipNearPlane(Camera camera, Portal portal)
        {
            float signDotProduct = Mathf.Sign(Vector3.Dot(portal.portalPlane.normal, portal.transform.position - camera.transform.position));
            Vector3 cameraSpacePos = camera.worldToCameraMatrix.MultiplyPoint(portal.transform.position);
            Vector3 cameraSpaceNormal = camera.worldToCameraMatrix.MultiplyVector(portal.portalPlane.normal) * signDotProduct;
            float cameraSpaceDistance = -Vector3.Dot(cameraSpacePos, cameraSpaceNormal) + 0.01f;
            return camera.CalculateObliqueMatrix(new Vector4(cameraSpaceNormal.x, cameraSpaceNormal.y, cameraSpaceNormal.z, cameraSpaceDistance));
        }
    }
}
