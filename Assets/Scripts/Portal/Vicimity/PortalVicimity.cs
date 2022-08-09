using System;
using System.Collections.Generic;
using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Portal
{
    [RequireComponent(typeof(Collider))]
    public class PortalVicimity : MonoBehaviour
    {
        private IPortal portal;
        
        private PortalVicimity portalPairVicimity;
        
        private readonly Dictionary<ITeleportable, NearbyTeleportable> vicimity = new();
        
        private void Start()
        {
            portal = GetComponentInParent<IPortal>();
            portalPairVicimity = portal.portalPair.transform.GetComponentInChildren<PortalVicimity>();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Set clip plane
            var objectRend = other.GetComponent<Renderer>();
            if (objectRend != null && objectRend.material.HasProperty("_ClipPlane"))
                objectRend.material.SetVector("_ClipPlane", portal.portalPlane.ToVector4());
            
            // Get teleportable (ignore if non-teleportable)
            var teleportable = other.GetComponentInParent<ITeleportable>();
            if(teleportable == null) 
                return;
            
            // Ignore if teleportable clone
            if(teleportable.transform.GetComponent<CloneBase>() != null)
                return;
            
            // If not in vicimity yet
            if (!vicimity.ContainsKey(teleportable))
            {
                // Create clone on other side of portal
                var clone = TeleportableClone.CreateClone(teleportable, portal);
                var nearbyTeleportable = new NearbyTeleportable(teleportable, clone);
                nearbyTeleportable.transferCallback = () =>
                {
                    TransferNearbyTeleportable(nearbyTeleportable);
                };
                
                // Transfer nearbyTeleportable to other portal vicimity when it teleports
                clone.OnEnterPortal += nearbyTeleportable.transferCallback;
                
                // Add to vicimity
                vicimity.Add(teleportable, nearbyTeleportable);
            }

            // Associate collider with teleportable
            var associatedColliders = vicimity[teleportable].associatedColliders;
            if (!associatedColliders.Contains(other))
                // Add collider to list
                associatedColliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            // Unset clip plane
            var objectRend = other.GetComponent<Renderer>();
            if (objectRend != null && objectRend.material.HasProperty("_ClipPlane"))
                objectRend.material.SetVector("_ClipPlane", Vector4.zero);
            
            // Get teleportable or ignore
            var teleportable = other.GetComponentInParent<ITeleportable>();
            if(teleportable == null) 
                return;
            
            // Ignore if teleportable clone
            if(teleportable.transform.GetComponent<CloneBase>() != null)
                return;
            
            // Match with vicimity object
            NearbyTeleportable nearbyTeleportable;
            if (vicimity.ContainsKey(teleportable))
                nearbyTeleportable = vicimity[teleportable];
            else // Not in vicimity (eg. after teleport)
                return;

            // Remove collider from list
            nearbyTeleportable.associatedColliders.Remove(other);
            
            // If no colliders remain, remove vicimity entry
            if (nearbyTeleportable.associatedColliders.Count == 0)
            {
                Destroy(nearbyTeleportable.clone.gameObject);
                vicimity.Remove(teleportable);
            }
        }

        private void TransferNearbyTeleportable(NearbyTeleportable nearbyTeleportable)
        {
            nearbyTeleportable.associatedColliders.Clear();
            portalPairVicimity.vicimity.Add(nearbyTeleportable.teleportable, nearbyTeleportable);
            vicimity.Remove(nearbyTeleportable.teleportable);
            nearbyTeleportable.clone.OnEnterPortal -= nearbyTeleportable.transferCallback;
            nearbyTeleportable.transferCallback = () =>
            {
                portalPairVicimity.TransferNearbyTeleportable(nearbyTeleportable); 
            };
            nearbyTeleportable.clone.OnEnterPortal += nearbyTeleportable.transferCallback;
        }
    }
}