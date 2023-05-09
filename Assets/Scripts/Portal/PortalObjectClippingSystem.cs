using MoreLinq;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Portals
{
    [RequireComponent(typeof(PortalVicinity), typeof(Portal))]
    public class PortalObjectClippingSystem: MonoBehaviourBase
    {
        [SerializeField] private float clipPlaneOffset;
        
        private static bool _registeredSwapCallback = false;
        private static readonly int ClipPlaneProperty = Shader.PropertyToID("_ClipPlane");
        private Vector4 portalPlaneVec;

        private void Start()
        {
            var vicinity = GetComponent<PortalVicinity>();
            var portal = GetComponent<Portal>();
            portalPlaneVec = portal.portalPlane.ToVector4();
            portalPlaneVec.w += clipPlaneOffset;

            vicinity.OnFrontToPass  += other =>   ClipByPlane(other, portalPlaneVec);

            vicinity.OnPassToFront  += other =>   ClipByPlane(other, Vector4.zero);

            vicinity.OnPassToInside += other =>   HideCompletely(other, true);

            vicinity.OnInsideToPass += other => { HideCompletely(other, false); ClipByPlane(other, portalPlaneVec); };
            
            vicinity.OnCloneIn += clone => clone.GetComponentsInChildren<Renderer>().ForEach( r => r.enabled = false);

            if (!_registeredSwapCallback)
            {
                GlobalEvents.OnSwap += OnSwapCallback;
                _registeredSwapCallback = true;
            }
        }
        
        private void ClipByPlane(Component component, Vector4 plane)
        {
            if (component.TryGetComponent(out Renderer renderer)) 
                renderer.material.SetVector(ClipPlaneProperty, plane);
        }
        
        private static void HideCompletely(Component component, bool hide)
        {
            if(component.TryGetComponent(out Renderer renderer))
                renderer.enabled = !hide;
        }
        
        private static void OnSwapCallback(SwapData swapData)
        {
            foreach (var renderSwap in swapData.rendererSwaps)
            {
                var item1ClipPlane = renderSwap.Item1.material.GetVector(ClipPlaneProperty);
                var item2ClipPlane = renderSwap.Item2.material.GetVector(ClipPlaneProperty);
                renderSwap.Item1.material.SetVector(ClipPlaneProperty, item2ClipPlane);
                renderSwap.Item2.material.SetVector(ClipPlaneProperty, item1ClipPlane);
                (renderSwap.Item1.enabled, renderSwap.Item2.enabled) = (renderSwap.Item2.enabled, renderSwap.Item1.enabled);
            }
        }
    }
}