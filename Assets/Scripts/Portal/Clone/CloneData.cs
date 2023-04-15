using UnityEngine;

namespace PerceptionVR.Portals
{
    public struct CloneData
    {
        public GameObject original;
        public GameObject clone;
        public Portal inPortal;
        public Portal outPortal;
        
        public CloneData(GameObject original, GameObject clone, Portal inPortal, Portal outPortal)
        {
            this.original = original;
            this.clone = clone;
            this.inPortal = inPortal;
            this.outPortal = outPortal;
        }
    }
}