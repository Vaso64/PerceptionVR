using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public class TransformClone : CloneBase<Transform>
    {
        private void Update()
        {
            if (targetSet)
                transform.SetPose(currentThroughPortal.PairPose(currentTarget.GetPose()));
        }
    }
}