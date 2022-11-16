using System;
using System.Linq;
using PerceptionVR.Common.Generic;
using PerceptionVR.Common;
using PerceptionVR.Global;
using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public class PortalCollisionFilteringSystem : MonoBehaviour
    {
        [SerializeField] protected SubscribableTrigger frontArea;
        [SerializeField] protected SubscribableTrigger passingArea;
        [SerializeField] protected SubscribableTrigger backArea;

        protected ColliderGroup frontGroup;
        protected ColliderGroup passingGroup;
        protected ColliderGroup backGroup;
        protected ColliderGroup cloneGroup;

        protected event Action<Collider> OnEnteredVicinity;
        protected event Action<Collider> OnExitedVicinity;

        protected IPortal portal;
        
        private PortalCollisionFilteringSystem pairCollisionFilteringSystem;
        
        private readonly TemporaryCollection<Collider> teleportedAwayLastFrame = new(FrameType.Fixed, 1);
        private readonly TemporaryCollection<Collider> teleportedInLastFrame = new(FrameType.Fixed, 1);
        

        protected virtual void Start()
        {
            portal = GetComponentInParent<IPortal>();
            pairCollisionFilteringSystem = portal.portalPair.transform.GetComponentInChildren<PortalCollisionFilteringSystem>();
            
            frontGroup = new ColliderGroup($"frontGroup_{portal.transform.name}");
            passingGroup = new ColliderGroup($"passingGroup_{portal.transform.name}");
            backGroup = new ColliderGroup($"backGroup_{portal.transform.name}");
            cloneGroup = new ColliderGroup($"cloneGroup_{portal.transform.name}");

            // Object passing through portals interact only with clone group and front group
            passingGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { cloneGroup, frontGroup });

            // Object inside the portal interact only with passing group
            cloneGroup.SetFilter(ColliderGroup.FilterMode.Include, new[] { passingGroup });
            
            // Add associated triggers to the collider groups
            frontGroup.Add(frontArea.collider);
            passingGroup.Add(passingArea.collider);
            backGroup.Add(backArea.collider);
            cloneGroup.Add(backArea.collider);

            frontArea.onTriggerEnter += OnFrontAreaEnter;
            frontArea.onTriggerExit += OnFrontAreaExit;
            passingArea.onTriggerEnter += OnPassingAreaEnter;
            passingArea.onTriggerExit += OnPassingAreaExit;
            backArea.onTriggerEnter += OnBackAreaEnter;
            backArea.onTriggerExit += OnBackAreaExit;
        }


        private void OnFrontAreaEnter(Collider other)
        {
            // Object teleported in
            if(teleportedInLastFrame.Contains(other))
                return;
            
            // Object entered from passing area (exiting portal)
            if(passingArea.Contains(other))
                return;
            
            // Object is in clone group
            if(cloneGroup.Contains(other))
                return;

            // Object entered from outside
            frontGroup.Add(other);
            OnEnteredVicinity?.Invoke(other);
        }

        private void OnFrontAreaExit(Collider other)
        {
            // Object teleported away
            if(teleportedAwayLastFrame.Contains(other))
                return;
            
            // Object exited to passing area (entering portal)
            if(passingArea.Contains(other))
                return;
            
            // Object is in clone group
            if(cloneGroup.Contains(other))
                return;

            // Object exited to outside
            frontGroup.Remove(other);
            OnExitedVicinity?.Invoke(other);
        }
        
        private void OnPassingAreaEnter(Collider other)
        {
            // Object teleported in
            if(teleportedInLastFrame.Contains(other))
                return;

            // Object entered from back area (clone exiting portal)
            if (backArea.Contains(other) && cloneGroup.Contains(other))
            {
                cloneGroup.Remove(other);
                passingGroup.Add(other);
            }

            // Object entered from front area (entering portal)
            else if (frontArea.Contains(other))
            {
                frontGroup.Remove(other);
                passingGroup.Add(other);
            }

            else
                Debugger.LogWarning($"{other.name} entered passing area from unknown source");
        }
        
        private void OnPassingAreaExit(Collider other)
        {
            // Object teleported away
            if(teleportedAwayLastFrame.Contains(other))
                return;
            
            // Object exited to back area (entering portal)
            if (backArea.Contains(other))
            {
                passingGroup.Remove(other);
                cloneGroup.Add(other);
            }

            // Object exited to front area (exiting portal)
            else if (frontArea.Contains(other))
            {
                passingGroup.Remove(other);
                frontGroup.Add(other);
            }

            else
                Debugger.LogWarning($"{other.name} exited passing area to unknown source");
        }
        
        private void OnBackAreaEnter(Collider other)
        {
            // Object entered from passing area
            if(passingArea.Contains(other) )
                return;
            
            // Object teleported in
            if(teleportedInLastFrame.Contains(other))
                return;
            
            // Object is from clone group
            if(cloneGroup.Contains(other))
                return;
            
            // Object is both in front and back area (not sure what to do yet)
            if (frontArea.Contains(other))
                return;

            // Object entered from outside area
            backGroup.Add(other);
        }
        
        private void OnBackAreaExit(Collider other)
        {
            // Object exited through passing area
            if(passingArea.Contains(other))
                return;
            
            // Object teleported away
            if(teleportedAwayLastFrame.Contains(other))
                return;
            
            // Object is from clone group
            if(cloneGroup.Contains(other))
                return;
            
            // Object exited to outside area
            backGroup.Remove(other);
        }

        protected void OnTeleport(NearbyObject nearbyObject)
        {
            // Swap collider groups
            var pairGroups = new[] { pairCollisionFilteringSystem.frontGroup, pairCollisionFilteringSystem.passingGroup, pairCollisionFilteringSystem.backGroup, pairCollisionFilteringSystem.cloneGroup };
            var thisGroups = new [] { frontGroup, passingGroup, backGroup, cloneGroup };

            foreach (var colliderPair in nearbyObject.pairLookup)
            {
                var currentGroupContainingOriginal = thisGroups.FirstOrDefault(x => x.Contains(colliderPair.original));
                var currentGroupContainingClone = pairGroups.FirstOrDefault(x => x.Contains(colliderPair.clone));

                if (currentGroupContainingClone != null)
                {
                    currentGroupContainingClone.Remove(colliderPair.clone);
                    currentGroupContainingClone.Add(colliderPair.original);
                }
                
                if (currentGroupContainingOriginal != null)
                {
                    currentGroupContainingOriginal.Remove(colliderPair.original);
                    currentGroupContainingOriginal.Add(colliderPair.clone);
                }
            }
            
            // Mark colliders as teleported
            foreach (var teleportableCollider in nearbyObject.associatedColliders)
            {
                teleportedAwayLastFrame.Add(teleportableCollider);
                pairCollisionFilteringSystem.teleportedInLastFrame.Add(teleportableCollider);
            }


            foreach (var cloneCollider in nearbyObject.associatedCloneColliders)
            {
                teleportedInLastFrame.Add(cloneCollider);
                pairCollisionFilteringSystem.teleportedAwayLastFrame.Add(cloneCollider);
            }
                
        }
    }
}