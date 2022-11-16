using System;
using UnityEngine;
using PerceptionVR.Extensions;

namespace PerceptionVR.Portal
{
    public class MovementClone : CloneBase<Transform>
    {
        private Rigidbody rb;
        private Rigidbody rbTarget;
        private enum TrackingType { TrackByRigidbody, TrackByTransform, TrackByLocalTransform }
        private TrackingType trackingType;

        private void Awake() => rb = GetComponent<Rigidbody>();

        public override void Track(Transform target, IPortal throughPortal)
        {
            rbTarget = target.GetComponent<Rigidbody>();
            if(rbTarget && !rbTarget.isKinematic)
                trackingType = TrackingType.TrackByRigidbody;
            else if (target.GetComponent<ITeleportable>() != null)
                trackingType = TrackingType.TrackByTransform;
            else
                trackingType = TrackingType.TrackByLocalTransform;
            base.Track(target, throughPortal);
        }
        
        private void FixedUpdate()
        {
            if(!targetSet || trackingType != TrackingType.TrackByRigidbody)
                return;
            
            var pose = rbTarget.transform.GetPose();
            
            // Heuristic next frame movement
            pose.position += rbTarget.velocity * Time.fixedDeltaTime;
            pose.rotation *= Quaternion.Euler(rbTarget.angularVelocity * Time.fixedDeltaTime);
            
            rb.VelocityPosition(throughPortal.PairPose(pose));
        }
        
        private void Update()
        {
            if(!targetSet)
                return;

            switch (trackingType)
            {
                case TrackingType.TrackByTransform:
                    transform.SetPose(throughPortal.PairPose(target.GetPose()));
                    break;
                
                case TrackingType.TrackByLocalTransform:
                    transform.SetLocalPose(target.GetLocalPose());
                    break;
            }
        }
    }
}