using System.Collections.Generic;
using PerceptionVR.Common;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public struct SwapData
    {
        public readonly (ISwappable, ISwappable) rootSwap;
        public readonly IEnumerable<(Collider, Collider)> colliderSwaps;
        public readonly IEnumerable<(Renderer, Renderer)> rendererSwaps;
        
        public SwapData(ISwappable original, ISwappable clone)
        {
            rootSwap = (original, clone);
            colliderSwaps = SwapUtility.CreateSwaps<Collider, Collider>(original.transform, clone.transform);
            rendererSwaps = SwapUtility.CreateSwaps<Renderer, Renderer>(original.transform, clone.transform);
        }
    }
}