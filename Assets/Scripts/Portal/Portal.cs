using System;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using PerceptionVR.Physics;


namespace PerceptionVR.Portal
{
    public partial class Portal : MonoBehaviour
    {
        [SerializeField] private Portal startingPortalPair;

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

        public void Teleport(ITeleportable teleportable)
        {
            var pairPose = PairPose(teleportable.transform.GetPose(), out var rotationDelta);

            // Teleport the object
            teleportable.transform.SetPositionAndRotation(pairPose.position, pairPose.rotation);
            
            // Translate velocity
            foreach (var teleportedRB in teleportable.transform.GetComponentsInChildren<Rigidbody>())
                teleportedRB.velocity = rotationDelta * teleportedRB.velocity;

            // Reorient the object
            foreach (var gravityObject in teleportable.transform.GetComponentsInChildren<IGravityObject>())
                gravityObject.gravityDirection = rotationDelta * gravityObject.gravityDirection;

            // Notify
            var teleportData = new TeleportData
            {
                teleportable = teleportable,
                inPortal = this,
                outPortal = portalPair,
                rotationDelta = rotationDelta
            };
            
            GlobalEvents.OnTeleport?.Invoke(teleportData);
            teleportable.OnTeleport?.Invoke(teleportData);
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


