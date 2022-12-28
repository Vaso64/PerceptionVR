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
        private Portal portal;
        
        [SerializeField] private Camera portalCamera;
        
        [SerializeField] private Renderer portalRend;

        private const int recurssionLimit = 3;

        private RenderTexture[] RTArray = new RenderTexture[recurssionLimit + 1];
        private Stack<int> RTArrayIndexStack = new Stack<int>(recurssionLimit);
        private bool RTArrayAlocated = false;

        private static List<PortalRenderer> allPortalRenderers = new();

        private void Awake()
        {
            RenderingManagment.OnResolutionChange += AllocateRTArray;
            
            // Get references
            portal ??= GetComponentInParent<Portal>();
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
            var oldProjection = portalCamera.projectionMatrix;
            portalCamera.projectionMatrix = ClipNearPlane(portalCamera, portal.portalPair);
            portalCamera.Render();
            //portalCamera.projectionMatrix = oldProjection;

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
