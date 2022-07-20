using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PerceptionVR.Portal
{
    [RequireComponent(typeof(Collider))]
    public class PortalVicimity : MonoBehaviour
    {
        private readonly List<NearbyObject> portalVicimity = new();
        
        private IPortal portal;

        private void Start()
        {
            portal = GetComponentInParent<IPortal>();
            StartCoroutine(VicimityTracking());
        }

        private IEnumerator VicimityTracking()
        {
            while (true)
            {
                foreach (var nearbyObject in portalVicimity)
                {
                    var currentDot = portal.PortalDot(nearbyObject.transform);

                    // Object passed through portal
                    if (currentDot < 0 && nearbyObject.portalDot > 0)
                        portal.Teleport(nearbyObject);

                    nearbyObject.portalDot = currentDot;  
                }
                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if object is teleportable
            var teleportable = other.GetComponent<ITeleportable>();
            if(teleportable == null) 
                return;
            
            // Start tracking
            Debug.Log($"{other.name} entered {transform.name} vicimity.");
            portalVicimity.Add(new NearbyObject(teleportable, other.transform, portal.PortalDot(other.transform)));

            // Set clip plane
            /*
            var objectRend = other.GetComponent<Renderer>();
            if (objectRend != null && objectRend.material.HasProperty("_ClipPlane"))
                objectRend.material.SetVector("_ClipPlane", new Vector4(portal.portalPlane.normal.x,
                    portal.portalPlane.normal.y,
                    portal.portalPlane.normal.z,
                    portal.portalPlane.distance));
            */
        }
        
        private void OnTriggerExit(Collider other)
        {
            // Portable object left the vicimity / got teleported
            if (other.GetComponent<ITeleportable>() != null)
            {
                Debug.Log($"{other.name} left {transform.name} vicimity.");
                portalVicimity.RemoveAll(x => x.transform == other.transform);
            }
        }
    }
}