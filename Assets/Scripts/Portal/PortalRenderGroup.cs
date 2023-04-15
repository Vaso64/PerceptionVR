using System.Collections;
using System.Collections.Generic;

namespace PerceptionVR.Portals
{
    public class PortalRenderGroup : MonoBehaviourBase, ICollection<PortalRenderer>
    {
        public List<PortalRenderer> portalRenderers;

        // ICollection implementation
        public IEnumerator<PortalRenderer> GetEnumerator() => portalRenderers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void Add(PortalRenderer item) => portalRenderers.Add(item);
        public void Clear() => portalRenderers.Clear();
        public bool Contains(PortalRenderer item) => portalRenderers.Contains(item);
        public void CopyTo(PortalRenderer[] array, int arrayIndex) => portalRenderers.CopyTo(array, arrayIndex);
        public bool Remove(PortalRenderer item) => portalRenderers.Remove(item);
        public int Count => portalRenderers.Count;
        public bool IsReadOnly => false;
    }
}