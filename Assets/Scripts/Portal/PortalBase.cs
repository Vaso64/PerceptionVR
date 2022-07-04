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

        public static Dictionary<Transform, OnTeleportDelegate> OnTeleport = new();

        public Transform portalPair;

        private List<VicimityObject> portalVicimity = new();

        private Plane portalPlane = new Plane();

        record VicimityObject
        {
            public VicimityObject(Transform transform, float dot)
            {
                this.transform = transform;
                this.dot = dot;
            }
            public Transform transform;
            public float dot;
        }

        protected virtual void Start()
        {
            StartCoroutine(VicimityTracking());
        }

        protected virtual void Update()
        {
            if (transform.hasChanged)
                portalPlane.SetNormalAndPosition(transform.forward, transform.position);
        }
        
        private IEnumerator VicimityTracking()
        {
            while (true)
            {
                foreach (var vicimityObject in portalVicimity)
                {
                    var currentDot = PortalDot(vicimityObject.transform);

                    // Object passed through portal
                    if (currentDot < 0 && vicimityObject.dot > 0)
                        Teleport(vicimityObject.transform);

                    vicimityObject.dot = currentDot;  
                }
                yield return null;
            }
        }
        
        private void Teleport(Transform teleportTransform)
        {
            Debug.Log($"{teleportTransform.name} passed through {transform.name}");

            Quaternion portalRotationDelta;
            var pairPose = CalculatePairPose(new Pose(teleportTransform.position, teleportTransform.rotation), out portalRotationDelta);

            teleportTransform.position = pairPose.position;
            teleportTransform.rotation = pairPose.rotation;

            // Notify teleported object
            if(OnTeleport.ContainsKey(teleportTransform))
                OnTeleport[teleportTransform]?.Invoke(portalRotationDelta);
        }

        
        public Pose CalculatePairPose(Pose pose) => CalculatePairPose(pose, out _);

        public Pose CalculatePairPose(Pose pose, out Quaternion portalRotationDelta)
        {
            Pose resultPose;
            portalRotationDelta = portalPair.rotation * Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(transform.rotation);

            // Calculate position
            resultPose.position = portalPair.position + portalRotationDelta * (pose.position - transform.position);

            // Rotation rotation
            resultPose.rotation = portalRotationDelta * pose.rotation;

            return resultPose;
        }



        private void OnTriggerEnter(Collider other)
        {
            // Portable object nearby
            if(MultiTag.ObjectHasTag(other, Tag.Teleportable))
            {
                // Start tracking
                Debug.Log($"{other.name} entered {transform.name} vicimity.");
                portalVicimity.Add(new VicimityObject(other.transform, PortalDot(other.transform)));

                // Set clip plane
                var objectRend = other.GetComponent<Renderer>();
                if (objectRend != null && objectRend.material.HasProperty("_ClipPlane"))
                    objectRend.material.SetVector("_ClipPlane", new Vector4(portalPlane.normal.x,
                                                                            portalPlane.normal.y,
                                                                            portalPlane.normal.z,
                                                                            portalPlane.distance));
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Portable object left the vicimity / got teleported
            if (MultiTag.ObjectHasTag(other, Tag.Teleportable))
            {
                Debug.Log($"{other.name} left {transform.name} vicimity.");
                portalVicimity.RemoveAll(x => x.transform == other.transform);
            }
        }

        private float PortalDot(Transform vicimityObject) => Vector3.Dot(transform.forward, transform.position - vicimityObject.position);
    }
}


