using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PerceptionVR.Common;

namespace PerceptionVR.Portal
{
    public class PortalBase : MonoBehaviour
    {
        public Transform portalPair;

        private Camera portalCamera;


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
            portalCamera = GetComponentInChildren<Camera>();
            portalCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
            gameObject.GetComponent<Renderer>().material.mainTexture = portalCamera.targetTexture;

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
                        Debug.Log($"{vicimityObject.transform.name} passed through {transform.name}");
                        Teleport(vicimityObject.transform);
                    }

                    vicimityObject.dot = currentDot;  
                }
                yield return null;
            }
        }
        
        private void Teleport(Transform transform)
        {
            var pairPose = CalculatePairPose(new Pose(transform.position, transform.rotation));
            
            transform.position = pairPose.position;
            transform.rotation = pairPose.rotation;
        }

        public Pose CalculatePairPose(Pose pose)
        {
            Pose pairPose;
            
            // Calculate delta
            Vector3 positionDelta = transform.position - pose.position;
            Quaternion portalRotationDelta = Quaternion.Inverse(portalPair.rotation) * transform.rotation;

            // Position camera
            pairPose.position = portalPair.position - portalRotationDelta * positionDelta;
            pairPose.rotation = portalRotationDelta * pose.rotation;

            return pairPose;
        }


        public void RenderFromPose(Pose pose)
        {
            var pairPose = CalculatePairPose(pose);

            portalCamera.transform.position = pairPose.position;
            portalCamera.transform.rotation = pairPose.rotation;

            // Render
            portalCamera.Render();
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


