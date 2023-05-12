using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Common.Generic;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public class PortalRenderGroup : MonoBehaviourCollection<PortalRenderer>
    {
        public IEnumerable<(PortalRenderer visiblerRenderer, Rect visibleArea)> GetVisible(Camera fromCamera, Rect fromArea)
        {
            var fromCameraFrustum = GeometryUtility.CalculateFrustumPlanes(fromCamera);
            Rect visibleArea = default;
            return this.Where(pr => pr.IsVisibleFrom(fromCamera, fromCameraFrustum, fromArea, out visibleArea))
                       .Select(pr => (pr, visibleArea));
        }
    }
}