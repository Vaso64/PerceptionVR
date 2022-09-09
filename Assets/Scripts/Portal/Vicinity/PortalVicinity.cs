﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Portal
{
    public class PortalVicinity : MonoBehaviour
    {
        [SerializeField] private SubscribableCollider frontArea;
        [SerializeField] private SubscribableCollider passingArea;
        [SerializeField] private SubscribableCollider backArea;
        
        private readonly ColliderGroup passingGroup = new();
        private readonly ColliderGroup backGroup = new();
        private readonly ColliderGroup cloneGroup = new();

        private IPortal portal;
        
        private PortalVicinity pairVicinity;
        
        private readonly Dictionary<ITeleportable, NearbyTeleportable> vicinity = new();
        
        private void Start()
        {
            portal = GetComponentInParent<IPortal>();
            portal.OnTeleport += OnTeleportCallback;
            pairVicinity = portal.portalPair.transform.GetComponentInChildren<PortalVicinity>();

            // Object passing through portals ignores things behind the portal
            passingGroup.SetFilter(ColliderGroup.FilterMode.Exclude, new[] { backGroup });

            // Cloned objects interact with only passing objects
            cloneGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { passingGroup });
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
            // Process teleportable (ignore if non-teleportable or clone)
            var teleportable = other.GetComponentInParent<ITeleportable>();
            if (teleportable != null)
            {
                // Skip clone
                if(teleportable.transform.GetComponent<CloneBase>() != null)
                    return;
                
                // If not in vicinity yet
                if (!vicinity.ContainsKey(teleportable))
                {
                    // Create clone on other side of portal
                    var clone = TeleportableClone.CreateClone(teleportable, portal);
                
                    // Add clone's colliders to pair's clone group
                    clone.GetComponentsInChildren<Collider>().ForEach(x => pairVicinity.cloneGroup.Add(x));
                    vicinity.Add(teleportable, new NearbyTeleportable(teleportable, clone));
                }

                // Associate collider with teleportable
                var associatedColliders = vicinity[teleportable].associatedColliders;
                if (!associatedColliders.Contains(other))
                    associatedColliders.Add(other);
            }
            
            // Process static collider
            else if (other.gameObject.isStatic)
                pairVicinity.cloneGroup.Add(CreateStaticClone(other));
        }

        private void OnFrontAreaExit(Collider other)
        {
            // Get teleportable (ignore if non-teleportable, clone or not in vicinity (after teleport))
            var teleportable = other.GetComponentInParent<ITeleportable>();
            if(teleportable == null || teleportable.transform.GetComponent<CloneBase>() != null || !vicinity.ContainsKey(teleportable)) 
                return;
            
            // Match with vicinity object and remove collider from it's list
            var nearbyTeleportable = vicinity[teleportable];
            nearbyTeleportable.associatedColliders.Remove(other);
            
            // If no colliders remains, remove vicinity entry
            if (nearbyTeleportable.associatedColliders.Count == 0)
            {
                GameObjectUtility.DestroyNotify(nearbyTeleportable.clone.gameObject);
                vicinity.Remove(teleportable);
            }
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            // TODO #2 - fix passingArea is overriding static clone geometry
            
            // Tempfix for #2
            if(other.gameObject.isStatic)
                return;
            
            // TODO #1 - Unsuccesful ColliderGroup.Remove might still cause unintentional ignores
            backGroup.Remove(other);
            cloneGroup.Remove(other);

            passingGroup.Add(other);
        }
        
        private void OnPassingAreaExit(Collider other)
        {
            // Tempfix for #2
            if(other.gameObject.isStatic)
                return;
            
            passingGroup.Remove(other);
            
            // If exited from back side, add to clone group
            if (backArea.Contains(other))
                cloneGroup.Add(other);
        }
        
        private void OnBackAreaEnter(Collider other)
        {
            if(!passingGroup.Contains(other) && !cloneGroup.Contains(other))
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
            pairVicinity.vicinity.Add(teleportable, nearbyTeleportable);
            vicinity.Remove(teleportable);
        }

        private Collider CreateStaticClone(Collider originalCollider)
        {
            // Instantiate
            var clone = GameObjectUtility.InstantiateNotify(originalCollider.gameObject);
            
            // TODO Proper component stripping
            clone.RemoveComponent<MeshFilter>();
            
            // Position
            clone.transform.SetPose(portal.PairPose(originalCollider.transform.GetPose()));
            
            return clone.GetComponent<Collider>();
        }
    }
}