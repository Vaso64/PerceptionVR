using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Common;

namespace PerceptionVR.Portal
{
    public class PortalBase : MonoBehaviour
    {
        public delegate void OnTeleportDelegate();

        public Dictionary<Transform, OnTeleportDelegate> OnTeleport = new Dictionary<Transform, OnTeleportDelegate>();

        public Transform portalPair;

        private List<VicimityObject> portalVicimity = new List<VicimityObject>();

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
        
        private IEnumerator VicimityTracking()
        {
            while (true)
            {
                foreach (var vicimityObject in portalVicimity)
                {
                    var currentDot = PortalDot(vicimityObject.transform);

                    // Object passed through portal
                    if (currentDot < 0 && vicimityObject.dot > 0)
                    {
                        
                        Teleport(vicimityObject.transform);
                    }

                    vicimityObject.dot = currentDot;  
                }
                yield return null;
            }
        }
        
        private void Teleport(Transform teleportTransform)
        {
            Debug.Log($"{teleportTransform.name} passed through {transform.name}");
            
            var pairPose = CalculatePairPose(new Pose(teleportTransform.position, teleportTransform.rotation));
            
            teleportTransform.position = pairPose.position;
            teleportTransform.rotation = pairPose.rotation;

            
        }

        public Pose CalculatePairPose(Pose pose)
        {
            Pose resultPose;
            
            // Calculate position
            Vector3 positionDelta = pose.position - transform.position;
            Quaternion portalRotationDelta = portalPair.rotation * Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(transform.rotation);
            resultPose.position = portalPair.position + portalRotationDelta * positionDelta;

            // Rotation rotation
            Quaternion rotationDelta = Quaternion.Euler(0, 180, 0) * Quaternion.Inverse(transform.rotation) * pose.rotation;
            resultPose.rotation = portalPair.rotation * rotationDelta;

            return resultPose;
        }




        private void OnTriggerEnter(Collider other)
        {
            // Portable object nearby
            if(MultiTag.ObjectHasTag(other, Tag.Teleportable))
            {
                Debug.Log($"{other.name} entered {transform.name} vicimity.");
                portalVicimity.Add(new VicimityObject(other.transform, PortalDot(other.transform)));
            }   
        }

        private void OnTriggerExit(Collider other)
        {
            // Portable object left the vicimity / got teleported
            if (MultiTag.ObjectHasTag(other, Tag.Teleportable))
            {
                portalVicimity.RemoveAll(x => x.transform == other.transform);
            }

        }

        private float PortalDot(Transform vicimityObject) => Vector3.Dot(transform.forward, transform.position - vicimityObject.position);
    }
}


