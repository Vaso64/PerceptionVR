using System.Collections.Generic;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Common;
using UnityEngine;
using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Debug;

namespace PerceptionVR.Portals
{
    [RequireComponent(typeof(Renderer))]
    public class PortalRenderer: MonoBehaviourBase
    {
        // References
        [SerializeField] private Camera portalCamera;
        [SerializeField] private float nearClipOffset = -0.0025f;
        
        
        private Portal portal;
        private Renderer portalRend;
        private Material portalRendSharedMat;
        private MaterialPropertyBlock portalPropBlock;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private const int RecursionLimit = 2;

        private Dictionary<DisplayMode, RenderTexture[]> RTArrays = new () {
            { DisplayMode.VR, new RenderTexture[RecursionLimit + 1] },
            { DisplayMode.Desktop, new RenderTexture[RecursionLimit + 1] }
        };
        private Dictionary<DisplayMode, Stack<int>> RTArraysIndexStack = new () {
            { DisplayMode.VR, new Stack<int>(RecursionLimit) },
            { DisplayMode.Desktop, new Stack<int>(RecursionLimit) }
        };
        
        public PortalRenderGroup renderGroup;
        public PortalRenderGroup pairRenderGroup;

        private void Awake()
        {
            // Allocate RTs
            AllocateRTArray(DisplayMode.Desktop, RenderingManagment.CurrentResolutions[DisplayMode.Desktop]);
            AllocateRTArray(DisplayMode.VR, RenderingManagment.CurrentResolutions[DisplayMode.VR]);
            RenderingManagment.OnResolutionChange += AllocateRTArray;
            
            // Get references
            portal = GetComponentInParent<Portal>();
            portalRend = GetComponent<Renderer>();
            portalRendSharedMat = portalRend.sharedMaterial;
            portalCamera ??= GetComponentInChildren<Camera>();
            portalPropBlock = new();
            renderGroup = GetComponentInParent<PortalRenderGroup>();

            portal.OnPortalPairSet += setPortal => EnablePortalRenderer(setPortal.GetComponentInChildren<PortalRenderer>()); 
            portal.OnPortalPairUnset += DisablePortalRenderer;
        }

        private void EnablePortalRenderer(PortalRenderer portalRendererPair)
        {
            renderGroup.Add(this);
            pairRenderGroup = portalRendererPair.renderGroup;
        }
        
        private void DisablePortalRenderer()
        {
            renderGroup.Remove(this);
            pairRenderGroup = null;
        }

        private void AllocateRTArray(DisplayMode displayMode, Vector2Int resolution)
        {
            if (resolution == Vector2Int.zero) return;
            for (var i = 0; i <= RecursionLimit; i++)
            {
                RTArrays[displayMode][i]?.Release();
                RTArrays[displayMode][i] = new RenderTexture(resolution.x, resolution.y, 24, RenderTextureFormat.Default);
            }
        }


        public void OnBeforePlayerCameraRenderCallback(Camera playerCamera, Plane[] playerFrustum)
        {
            if (IsVisibleFrom(playerCamera, playerFrustum, out var visibleArea))
            {
                RenderPortal(playerCamera.transform.GetPose(), visibleArea, playerCamera.fieldOfView, playerCamera.GetDisplayMode(), 0);
                portalRendSharedMat.mainTextureOffset = playerCamera.rect.position;
                portalRendSharedMat.mainTextureScale = playerCamera.rect.size;
            }
        }


        private void OnAfterPortalRenderCallback(DisplayMode displayMode)
        {
            // Set portal to previous texture after being rendered
            RTArraysIndexStack[displayMode].Pop();
            if (RTArraysIndexStack[displayMode].Count > 0)
                portalPropBlock.SetTexture(MainTex, RTArrays[displayMode][RTArraysIndexStack[displayMode].Peek()]);
            portalRend.SetPropertyBlock(portalPropBlock);
        }

        // Recursively renders portals
        private void RenderPortal(Pose fromPose, Rect visibleArea, float fov, DisplayMode displayMode, int recursionDepth)
        {
            // Setup camera
            portalCamera.ResetProjectionMatrix();
            portalCamera.rect = new Rect(0, 0, 1, 1);
            portalCamera.fieldOfView = fov;
            portalCamera.targetTexture = RTArrays[displayMode][recursionDepth]; // Set's camera resolution
            var pairPose = portal.PairPose(fromPose);
            portalCamera.transform.SetPose(pairPose);
            portalCamera.SetNearPlane(portal.portalPair.portalPlane, offset: nearClipOffset);
            portalCamera.SetScissorRect(visibleArea);
            var pm = portalCamera.projectionMatrix;

            // Get visible portals / recurse
            (PortalRenderer, Rect)[] visiblePortalRenderers = null;
            if (recursionDepth < RecursionLimit)
            {
                Rect prVisibleArea = default;
                var portalCameraFrustum = GeometryUtility.CalculateFrustumPlanes(portalCamera);
                visiblePortalRenderers = pairRenderGroup
                    .Where(pr => pr.IsVisibleFrom(portalCamera, portalCameraFrustum, out prVisibleArea))
                    .Select(pr => (pr, prVisibleArea))
                    .ToArray();
            }

            // Render visible
            visiblePortalRenderers?.ForEach(x => x.Item1.RenderPortal(pairPose, x.Item2, portalCamera.fieldOfView, displayMode, recursionDepth + 1));

            // Render self
            portalCamera.transform.SetPose(pairPose);
            portalCamera.targetTexture = RTArrays[displayMode][recursionDepth];
            portalCamera.projectionMatrix = pm;
            portalCamera.rect = visibleArea;
            portalRendSharedMat.mainTextureOffset = visibleArea.position;
            portalRendSharedMat.mainTextureScale = visibleArea.size;
            portalCamera.Render();
            portalCamera.ResetProjectionMatrix();
            RenderingManagment.portalRenderCount++;
            
            // Notify after render
            visiblePortalRenderers?.ForEach(x => x.Item1.OnAfterPortalRenderCallback(displayMode));

            // Display RT
            RTArraysIndexStack[displayMode].Push(recursionDepth);
            portalPropBlock.SetTexture(MainTex, portalCamera.targetTexture);
            portalRend.SetPropertyBlock(portalPropBlock);
        }


        // Returns true if the portal is visible from the camera 
        private bool IsVisibleFrom(Camera fromCamera, Plane[] fromFrustum, out Rect visibleArea)
        {
            visibleArea = Rect.zero;
            
            // Dot product check (is camera behind the portal?)
            if (Vector3.Dot(transform.forward, transform.position - fromCamera.transform.position) < 0)
                return false;
            
            // AABB frustum check (is portal in fov of the camera?)
            if (!GeometryUtility.TestPlanesAABB(fromFrustum, portal.portalCollider.bounds))
                return false;
            
            // Overlap edge check (portals overlapping but only on the edge => viewport less than one pixel)
            // Not necessary but unity complains when rendering at 0 resolution
            visibleArea = fromCamera.WorldToViewportBounds(portal.portalCollider.bounds).Scale(fromCamera.rect).Clamp(fromCamera.rect);
            var screenSize = fromCamera.targetTexture != null ? new Vector2(fromCamera.targetTexture.width, fromCamera.targetTexture.height) 
                                                              : new Vector2(fromCamera.pixelWidth, fromCamera.pixelHeight);
            if (visibleArea.width * screenSize.x < 1f || visibleArea.height * screenSize.y < 1f)
                return false;

            return true;
        }
    }
}
