using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using PerceptionVR.Extensions;
using PerceptionVR.Physics;
using UnityEngine;


namespace PerceptionVR.Portals
{
    [RequireComponent(typeof(Portal), typeof(PortalVicinity), typeof(PortalCollisionFilteringSystem))]
    public class PortalCloneSystem : MonoBehaviour
    {
        [SerializeField] private List<NearbyTeleportable> objectsInVicinity = new();
        
        private Portal portal;
        private PortalVicinity vicinity;
        private PortalCollisionFilteringSystem collisionFilteringSystem;
        private PortalCloneSystem pairCloneSystem;
        
        private void Awake()
        {
            portal = GetComponent<Portal>();
            vicinity = GetComponent<PortalVicinity>();
            collisionFilteringSystem = GetComponent<PortalCollisionFilteringSystem>();
            
            portal.OnPortalPairSet += portalPair =>
            {
                pairCloneSystem = portalPair.transform.GetComponentInChildren<PortalCloneSystem>();
            };
            portal.OnPortalPairUnset += () =>
            {
                objectsInVicinity.ToList().ForEach(UnregisterTeleportable);
                pairCloneSystem = null;
            };
            
            vicinity.OnOutsideToFront += OnVicinityEnter;
            vicinity.OnFrontToOutside += OnVicinityExit;
            
            vicinity.OnBackToFront    += OnVicinityEnter;
            vicinity.OnFrontToBack    += OnVicinityExit;
            
            GlobalEvents.OnTeleport += OnTeleportCallback;
        }


        private void OnVicinityEnter(Collider other)
        {
            // Ignore if clone is coming around the portal
            if(collisionFilteringSystem.cloneGroup.Contains(other))
                return;
            
            Debugger.LogInfo($"{other} entered vicinity of {this}");
            var tp = other.GetComponentInParent<ITeleportable>();
            
            // Get nearby teleportable
            var nearbyTeleportable = objectsInVicinity.FirstOrDefault(x => x.teleportable == tp);
            if (nearbyTeleportable != null)
            {
                nearbyTeleportable.collidersInVicinity.Add(other);
                return;
            }
            
            nearbyTeleportable = pairCloneSystem.objectsInVicinity.FirstOrDefault(x => x.cloneTeleportable == tp);
            if (nearbyTeleportable != null)
            {
                nearbyTeleportable.cloneColliderInPairVicinity.Add(other);
                return;
            }    
            
            // Don't clone clones (yet)
            if(other.GetComponentInParent<TeleportableClone>() != null)
                return;
            
            RegisterTeleportable(tp).collidersInVicinity.Add(other);
        }
    
        private void OnVicinityExit(Collider other)
        {
            // Ignore if clone is coming around the portal
            if(collisionFilteringSystem.cloneGroup.Contains(other))
                return;
            
            Debugger.LogInfo($"{other} exited vicinity of {this}");
            var tp = other.GetComponentInParent<ITeleportable>();
            
            var nearbyTeleportable = objectsInVicinity.FirstOrDefault(x => x.teleportable == tp);
            if (nearbyTeleportable != null)
            {
                nearbyTeleportable.collidersInVicinity.Remove(other);
                if(nearbyTeleportable.collidersInVicinity.Count == 0)
                    UnregisterTeleportable(nearbyTeleportable);
                return;
            }
            
            nearbyTeleportable = pairCloneSystem.objectsInVicinity.FirstOrDefault(x => x.cloneTeleportable == tp);
            if (nearbyTeleportable != null)
                nearbyTeleportable.cloneColliderInPairVicinity.Remove(other);
        }
        
        private NearbyTeleportable RegisterTeleportable(ITeleportable teleportable)
        {
            Debugger.LogInfo($"Registering {teleportable} in {this}");
            
            // Create clone
            var clone = CreateClone(teleportable, trackedPortal: portal) as ITeleportable;
            
            // Register teleportable
            var nearbyTeleportable = new NearbyTeleportable(teleportable, clone);
            objectsInVicinity.Add(nearbyTeleportable);
            
            // Add clone colliders to nearbyTeleportable and pair's cloneGroup
            var cloneColliders = clone.transform.GetComponentsInChildren<Collider>();
            nearbyTeleportable.cloneColliderInPairVicinity.AddRange(cloneColliders);
            pairCloneSystem.collisionFilteringSystem.cloneGroup.AddRange(cloneColliders);
            
            return nearbyTeleportable;
        }
        
        private void UnregisterTeleportable(NearbyTeleportable teleportable)
        { 
            Debugger.LogInfo($"Unregistering {teleportable} from {this}");
            
            // Destroy clone
            GameObjectUtility.DestroyNotify(teleportable.cloneTeleportable.gameObject);
            
            // Unregister teleportable
            objectsInVicinity.Remove(teleportable);
        }


        private void OnTeleportCallback(TeleportData teleportData)
        {
            // nt = "nearby teleportable"
            
            var nt = objectsInVicinity.FirstOrDefault(x => x.teleportable == teleportData.teleportable && portal == teleportData.inPortal);
            if(nt == null)
                return;

            var swapData = new SwapData(nt.teleportable, nt.cloneTeleportable);
            
            // Notify swap
            GlobalEvents.OnSwap?.Invoke(swapData);

            Debugger.LogInfo($"Swapping teleportables of {swapData.teleportableSwap.Item1} and {swapData.teleportableSwap.Item2} in {this}");

            // Swap colliders
            SwapUtility.PerformSwap(nt.collidersInVicinity, swapData.colliderSwaps);
            SwapUtility.PerformSwap(nt.cloneColliderInPairVicinity, swapData.colliderSwaps);
            
            // Swap collider lists
            (nt.collidersInVicinity, nt.cloneColliderInPairVicinity) = (nt.cloneColliderInPairVicinity, nt.collidersInVicinity);

            // Transfer nt to pair
            objectsInVicinity.Remove(nt);
            pairCloneSystem.objectsInVicinity.Add(nt);
        }
        
        
        private TeleportableClone CreateClone(ITeleportable original, Portal trackedPortal)
        {
            // Clone original
            var clone = GameObjectUtility.InstantiateNotify(original.gameObject);
            clone.transform.name = clone.transform.name.Replace("(Clone)", "");

            IEnumerable<Type> defaultPreservedComponents = new [] { typeof(Rigidbody), typeof(Collider), typeof(MeshRenderer), 
                                                                    typeof(MeshFilter), typeof(Transform) };

            
            // Strip joints immediately so it won't mess with physics
            foreach (var cloneJoint in clone.GetComponentsInChildren<TeleportableJoint>().Cast<Component>().Concat(clone.GetComponentsInChildren<Joint>()))
                GameObjectUtility.DestroyComponentImmediateNotify(cloneJoint);

            // Iterate through all children
            foreach (var child in clone.GetComponentsInChildren<Transform>())
            {
                var childComponents = child.GetComponents<Component>().ToList();
                var preservedComponents = defaultPreservedComponents;
                
                // Try get TeleportableBehaviour
                var tb = childComponents.OfType<ITeleportableBehaviour>().FirstOrDefault();
                if (tb != null)
                {
                    // Notify teleportableBehaviour
                    tb.OnCreateClone(child.gameObject, out var extraPreservedComponents);
                    preservedComponents = preservedComponents.Concat(extraPreservedComponents);
                }
                
                // Strip unneeded components
                foreach (var component in childComponents.ToArray())
                {
                    var componentType = component.GetType();
                    if (!preservedComponents.Any(preserveComp => preserveComp == componentType || componentType.IsSubclassOf(preserveComp)))
                    {
                        GameObjectUtility.DestroyComponentNotify(component);
                        childComponents.Remove(component);
                    }
                }
                
                // Set-up Movement clone
                child.gameObject.AddComponentNotify<MovementClone>();
                
                // Set-up TeleportableBehaviour clone
                if(tb != null)
                    child.gameObject.AddComponentNotify<TeleportableBehaviourClone>();
                
                child.name += " (Clone)";
            }

            var teleportableClone = clone.AddComponentNotify<TeleportableClone>();
            teleportableClone.Track(original, trackedPortal);
            
            Debugger.LogInfo($"{original.transform} was cloned.");

            return teleportableClone;
        }
    }
    
}