using MoreLinq;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Portals
{
    [RequireComponent(typeof(PortalVicinity), typeof(Portal))]
    public class PortalObjectClippingSystem: MonoBehaviourBase
    {
        private static bool _registeredSwapCallback = false;
        private static readonly int ClipPlane = Shader.PropertyToID("_ClipPlane");

        private void Start()
        {
            var vicinity = GetComponent<PortalVicinity>();
            var portal = GetComponent<Portal>();
            
            var zeroPlane = new Plane(Vector3.zero, 0);

            vicinity.OnFrontToPass  += other =>   ClipByPlane(other, portal.portalPlane);

            vicinity.OnPassToFront  += other =>   ClipByPlane(other, zeroPlane);

            vicinity.OnPassToInside += other =>   HideCompletely(other, true);

            vicinity.OnInsideToPass += other => { HideCompletely(other, false); ClipByPlane(other, portal.portalPlane); };
            
            vicinity.OnCloneIn += clone => clone.GetComponentsInChildren<Renderer>().ForEach( r => r.enabled = false);

            if (!_registeredSwapCallback)
            {
                GlobalEvents.OnSwap += OnSwapCallback;
                _registeredSwapCallback = true;
            }
        }
        
        private static void ClipByPlane(Component component, Plane plane)
        {
            if (component.TryGetComponent(out Renderer renderer))
                renderer.material.SetVector(ClipPlane, plane.ToVector4());
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
                var item1ClipPlane = renderSwap.Item1.material.GetVector(ClipPlane);
                var item2ClipPlane = renderSwap.Item2.material.GetVector(ClipPlane);
                renderSwap.Item1.material.SetVector(ClipPlane, item2ClipPlane);
                renderSwap.Item2.material.SetVector(ClipPlane, item1ClipPlane);
                (renderSwap.Item1.enabled, renderSwap.Item2.enabled) = (renderSwap.Item2.enabled, renderSwap.Item1.enabled);
            }
        }
    }
}