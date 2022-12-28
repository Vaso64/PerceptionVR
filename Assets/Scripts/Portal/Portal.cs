using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Common;
using PerceptionVR.Extensions;
using UnityEngine.Serialization;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using PerceptionVR.Physics;


namespace PerceptionVR.Portal
{
    public partial class Portal : MonoBehaviour
    {
        [SerializeField] private Portal startingPortalPair;
        
        public event Action<TeleportData> OnTeleport;
        
        public Collider portalCollider { get; private set; }

        public Plane portalPlane => new(transform.forward, transform.position);
        

        private void Awake()
        {
            portalCollider = GetComponent<Collider>();
        }

        private void Start()
        {
            // Portal pair checks
            if (startingPortalPair != null)
                SetPortalPair(this, startingPortalPair);
        }

        public void Teleport(TeleportData teleportData)
        {
            var pairPose = PairPose(teleportData.teleportable.transform.GetPose(), out teleportData.rotationDelta);

            // Teleport the object
            teleportData.teleportable.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            
            // Translate velocity
            foreach (var teleportedRB in teleportData.teleportable.transform.GetComponentsInChildren<Rigidbody>())
                teleportedRB.velocity = teleportData.rotationDelta * teleportedRB.velocity;

            // Reorient the object
            foreach (var gravityObject in teleportData.teleportable.transform.GetComponentsInChildren<IGravityObject>())
                gravityObject.gravityDirection = teleportData.rotationDelta * gravityObject.gravityDirection;

            // Notify
            GlobalEvents.OnTeleport?.Invoke(teleportData);
            teleportData.teleportable.OnTeleport(teleportData);
            OnTeleport?.Invoke(teleportData);
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


