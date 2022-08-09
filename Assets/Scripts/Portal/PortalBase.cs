using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Common;
using PerceptionVR.Extensions;

namespace PerceptionVR.Portal
{
    public class PortalBase : MonoBehaviour
    {
        [SerializeField] private PortalBase _portalPair;
        
        [SerializeField] private Collider teleportableArea;
        
        public IPortal portalPair => _portalPair as IPortal;

        public Action<ITeleportable> OnTeleport;

        public Plane portalPlane => new Plane(transform.forward, transform.position);

        public Bounds teleportableBounds => teleportableArea.bounds;

        private void Awake()
        {
            // Portal pair checks
            if (portalPair == null) 
                Debug.LogError("PortalBase: No portal pair assigned");
            if (_portalPair.transform == transform)
                Debug.LogError("PortalBase: Portal pair cannot be self");
        }

        public void Teleport(ITeleportable teleportable)
        {
            //Debug.Log($"{teleportable.transform.name} passed through {transform.name}");
            
            var pairPose = PairPose(teleportable.transform.GetPose(), out var portalRotationDelta);

            // Teleport the object
            teleportable.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            
            // Translate velocity
            var nearbyObjectRB = teleportable.transform.GetComponent<Rigidbody>();
            if (nearbyObjectRB != null)
                nearbyObjectRB.velocity = portalRotationDelta * nearbyObjectRB.velocity;
            
            // Notify teleported object
            teleportable.OnTeleport(new TeleportData(portalRotationDelta));
            OnTeleport?.Invoke(teleportable);
        }

        
        public virtual Pose PairPose(Pose pose) => PairPose(pose, out _);

        public virtual Pose PairPose(Pose pose, out Quaternion portalRotationDelta)
        {
            Pose resultPose;
            portalRotationDelta = portalPair.transform.rotation * Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(transform.rotation);

            // Calculate position
            resultPose.position = portalPair.transform.position + portalRotationDelta * (pose.position - transform.position);

            // Rotation rotation
            resultPose.rotation = portalRotationDelta * pose.rotation;

            return resultPose;
        }
    }
}


