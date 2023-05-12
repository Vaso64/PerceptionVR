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
        
        private readonly Stack<RenderTexture> rendererTexturesStack = new();
        private DisplayMode lastDisplayMode;

        public PortalRenderGroup renderGroup { get; private set; }
        private PortalRenderGroup pairRenderGroup;

        private void Awake()
        {
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
        
        
        // Start portal recursion chain
        public void StartRenderPortalChain(Camera fromCamera, Rect visibleArea)
        {
            RenderPortal(fromCamera.transform.GetPose(), visibleArea, fromCamera.fieldOfView, fromCamera.GetDisplayMode(), 0);
            portalRendSharedMat.mainTextureOffset = fromCamera.rect.position; 
            portalRendSharedMat.mainTextureScale = fromCamera.rect.size;
        } 

        
        // Single render command
        private void RenderPortal(Pose fromPose, Rect visibleArea, float fov, DisplayMode displayMode, int recursionDepth)
        {
            // Setup camera
            lastDisplayMode = displayMode;
            portalCamera.ResetProjectionMatrix();
            portalCamera.rect = new Rect(0, 0, 1, 1);
            portalCamera.fieldOfView = fov;
            portalCamera.targetTexture = RenderingManagment.PeekRTPool(displayMode); // Sets resolution
            var pairPose = portal.PairPose(fromPose);
            portalCamera.transform.SetPose(pairPose);
            portalCamera.SetNearPlane(portal.portalPair.portalPlane, offset: nearClipOffset);
            portalCamera.SetScissorRect(visibleArea);
            var pm = portalCamera.projectionMatrix;

            // Get visible portals
            var visiblePortalRenderers = new (PortalRenderer visibleRenderer, Rect visibleArea)[] {};
            if (recursionDepth < RenderingManagment.PortalRecursionLimit)
                visiblePortalRenderers = pairRenderGroup.GetVisible(portalCamera).ToArray();

            // Render visible (recurse)
            visiblePortalRenderers.ForEach(x => x.visibleRenderer.RenderPortal(pairPose, x.visibleArea, fov, displayMode, recursionDepth + 1));

            // Render self
            portalCamera.transform.SetPose(pairPose);
            portalCamera.projectionMatrix = pm;
            portalCamera.rect = visibleArea;
            portalRendSharedMat.mainTextureOffset = visibleArea.position;
            portalRendSharedMat.mainTextureScale = visibleArea.size;
            portalCamera.targetTexture = RenderingManagment.GetRTFromPool(displayMode);
            portalCamera.Render();
            portalCamera.ResetProjectionMatrix();
            RenderingManagment.portalRenderCount++;
            
            // Notify after render
            visiblePortalRenderers.ForEach(x => x.visibleRenderer.OnAfterParentRender());

            // Display RT
            rendererTexturesStack.Push(portalCamera.targetTexture);
            portalPropBlock.SetTexture(MainTex, portalCamera.targetTexture);
            portalRend.SetPropertyBlock(portalPropBlock);
        }
        
        
        // Gets called after being rendered
        public void OnAfterParentRender()
        {
            // Return RT to shared pool
            RenderingManagment.ReturnRTToPool(lastDisplayMode, rendererTexturesStack.Pop());

            // Set portal to previous texture
            if (rendererTexturesStack.Count > 0)
                portalPropBlock.SetTexture(MainTex, rendererTexturesStack.Peek());
            portalRend.SetPropertyBlock(portalPropBlock);
        }


        // Returns true if the portal is visible from the camera 
        public bool IsVisibleFrom(Camera fromCamera, Plane[] fromFrustum, out Rect visibleArea)
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
