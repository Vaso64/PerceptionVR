using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public class LocalTransformClone : CloneBase<Transform>
    {
        private void Update()
        {
            if (!targetSet) 
                return;
            
            transform.localPosition = currentTarget.localPosition;
            transform.localRotation = currentTarget.localRotation;
        }
    }
}