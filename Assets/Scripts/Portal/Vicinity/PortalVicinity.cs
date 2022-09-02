using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Portal
{
    public class PortalVicinity : MonoBehaviour
    {
        [SerializeField] private SubscribableCollider frontArea;
        [SerializeField] private SubscribableCollider passingArea;
        [SerializeField] private SubscribableCollider backArea;
        
        private readonly ColliderGroup passingGroup = new ColliderGroup();
        private readonly ColliderGroup backGroup = new ColliderGroup();
        private readonly ColliderGroup backCloneGroup = new ColliderGroup();

        private IPortal portal;
        
        private Dictionary<ITeleportable, NearbyTeleportable> pairVicinity;
        
        private readonly Dictionary<ITeleportable, NearbyTeleportable> vicinity = new();
        
        private void Start()
        {
            portal = GetComponentInParent<IPortal>();
            portal.OnTeleport += OnTeleportCallback;
            pairVicinity = portal.portalPair.transform.GetComponentInChildren<PortalVicinity>().vicinity;

            // Object passing through portals ignores things behind the portal
            passingGroup.SetFilter(ColliderGroup.FilterMode.Exclude, backGroup);

            // Cloned objects interact with only passing objects
            backCloneGroup.SetFilter(ColliderGroup.FilterMode.Include, passingGroup);
            passingGroup.Add(passingArea.collider);

            frontArea.onTriggerEnter += OnFrontAreaEnter;
            frontArea.onTriggerExit += OnFrontAreaExit;
            passingArea.onTriggerEnter += OnPassingAreaEnter;
            passingArea.onTriggerExit += OnPassingAreaExit;
            backArea.onTriggerEnter += OnBackAreaEnter;
            backArea.onTriggerExit += OnBackAreaExit;
        }


        private void OnFrontAreaEnter(Collider other)
        {
            // Get teleportable (ignore if non-teleportable or clone)
            var teleportable = other.GetComponentInParent<ITeleportable>();
            if(teleportable == null || teleportable.transform.GetComponent<CloneBase>() != null) 
                return;

            // If not in vicinity yet
            if (!vicinity.ContainsKey(teleportable))
            {
                // Create clone on other side of portal and add to vicinity
                var clone = TeleportableClone.CreateClone(teleportable, portal);
                vicinity.Add(teleportable, new NearbyTeleportable(teleportable, clone));
            }

            // Associate collider with teleportable
            var associatedColliders = vicinity[teleportable].associatedColliders;
            if (!associatedColliders.Contains(other))
                associatedColliders.Add(other);
        }

        private void OnFrontAreaExit(Collider other)
        {
            // Get teleportable (ignore if non-teleportable, clone or not in vicinity (after teleport))
            var teleportable = other.GetComponentInParent<ITeleportable>();
            if(teleportable == null || teleportable.transform.GetComponent<CloneBase>() != null || !vicinity.ContainsKey(teleportable)) 
                return;
            
            // Match with vicinity object and remove collider from it's list
            NearbyTeleportable nearbyTeleportable = vicinity[teleportable];
            nearbyTeleportable.associatedColliders.Remove(other);
            
            // If no colliders remains, remove vicinity entry
            if (nearbyTeleportable.associatedColliders.Count == 0)
            {
                Destroy(nearbyTeleportable.clone.gameObject);
                vicinity.Remove(teleportable);
            }
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            backGroup.Remove(other);
            backCloneGroup.Remove(other);
            passingGroup.Add(other);
        }
        
        private void OnPassingAreaExit(Collider other)
        {
            passingGroup.Remove(other);
            
            // If exited from front side
            if (frontArea.Contains(other))
                return;
            
            // other Should be a clone at this point
            if(other.GetComponentInParent<TeleportableClone>() != null)  
                backCloneGroup.Add(other);
        }
        
        private void OnBackAreaEnter(Collider other)
        {
            if(!passingGroup.Contains(other))
                backGroup.Add(other);
        }
        
        private void OnBackAreaExit(Collider other)
        {
            backGroup.Remove(other);
        }

        private void OnTeleportCallback(ITeleportable teleportable)
        {
            var nearbyTeleportable = vicinity[teleportable];
            nearbyTeleportable.associatedColliders.Clear();
            pairVicinity.Add(teleportable, nearbyTeleportable);
            vicinity.Remove(teleportable);
        }
    }
}