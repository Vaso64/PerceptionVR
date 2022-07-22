using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Common;

namespace PerceptionVR.Portal
{
    public class PortalBase : MonoBehaviour
    {
        public delegate void OnTeleportDelegate(Quaternion portalDelta);
        

        public Transform portalPair;


        public Plane portalPlane { get; protected set; }

        private void Awake()
        {
            // Portal pair checks
            if (portalPair == null) 
                Debug.LogError("PortalBase: No portal pair assigned");
            if (portalPair == transform)
                Debug.LogError("PortalBase: Portal pair cannot be self");
        }

        protected virtual void Update()
        {
            if (transform.hasChanged)
                portalPlane.SetNormalAndPosition(transform.forward, transform.position);
        }

        public void Teleport(NearbyObject nearbyObject)
        {
            Debug.Log($"{nearbyObject.teleportable.transform.name} passed through {transform.name}");
            
            var pairPose = PairPose(new Pose(nearbyObject.teleportable.transform.position, nearbyObject.teleportable.transform.rotation), out var portalRotationDelta);

            // Teleport the object
            nearbyObject.teleportable.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            
            // Translate velocity
            var nearbyObjectRB = nearbyObject.teleportable.transform.GetComponent<Rigidbody>();
            if (nearbyObjectRB != null)
                nearbyObjectRB.velocity = portalRotationDelta * nearbyObjectRB.velocity;

                // Notify teleported object
            nearbyObject.teleportable.OnTeleport(new TeleportData(portalRotationDelta));
        }

        
        public virtual Pose PairPose(Pose pose) => PairPose(pose, out _);

        public virtual Pose PairPose(Pose pose, out Quaternion portalRotationDelta)
        {
            Pose resultPose;
            portalRotationDelta = portalPair.rotation * Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(transform.rotation);

            // Calculate position
            resultPose.position = portalPair.position + portalRotationDelta * (pose.position - transform.position);

            // Rotation rotation
            resultPose.rotation = portalRotationDelta * pose.rotation;

            return resultPose;
        }

        public float PortalDot(Transform nearbyObject) => Vector3.Dot(transform.forward, transform.position - nearbyObject.position);
    }
}


