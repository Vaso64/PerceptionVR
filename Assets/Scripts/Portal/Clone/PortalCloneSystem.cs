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
    [RequireComponent(typeof(Portal), typeof(PortalVicinity))]
    public class PortalCloneSystem: MonoBehaviourBase
    {
        [SerializeField] private List<NearbyTeleportable> objectsInVicinity = new();
        
        private Portal portal;
        private PortalVicinity vicinity;
        private PortalCloneSystem pairCloneSystem;
        
        private void Awake()
        {
            portal = GetComponent<Portal>();
            vicinity = GetComponent<PortalVicinity>();

            portal.OnPortalPairSet   += portalPair => pairCloneSystem = portalPair.transform.GetComponentInChildren<PortalCloneSystem>();
            portal.OnPortalPairUnset += () =>
            {
                objectsInVicinity.ToList().ForEach(UnregisterTeleportable);
                pairCloneSystem = null;
            };
            
            vicinity.OnLargeToFront += OnVicinityEnter;
            vicinity.OnFrontToLarge += OnVicinityExit;

            GlobalEvents.OnTeleport += OnTeleportCallback;
        }


        private void OnVicinityEnter(Collider other)
        {
            Debugger.LogInfo($"{other} entered vicinity of {this}");
            
            // Collider belongs to teleportable
            if (other.GetComponentInParent<TeleportableObject>() is { } tp)
            {
                // Teleportable is registered in vicinity
                if(objectsInVicinity.FirstOrDefault(x => x.teleportableObject == tp) is { } nearbyTeleportable)
                    nearbyTeleportable.collidersInVicinity.Add(other);

                // Teleportable not yet registered in vicinity
                else
                    RegisterTeleportable(tp).collidersInVicinity.Add(other);
            }
            
            // Collider belongs to teleportable clone
            else if (other.GetComponentInParent<TeleportableObjectClone>() is { } tpClone)
            {
                // Clone is registered in pair's vicinity
                if (pairCloneSystem.objectsInVicinity.FirstOrDefault(x => x.cloneTeleportableObject == tpClone) is { } nearbyPairTeleportable)
                    nearbyPairTeleportable.cloneColliderInPairVicinity.Add(other);
            }
            
            // Unknown collider
            else 
                Debugger.LogWarning($"Collider {other} is not neither teleportable or teleportable clone");
        }
    
        private void OnVicinityExit(Collider other)
        {
            Debugger.LogInfo($"{other} exited vicinity of {this}");
            var tp = other.GetComponentInParent<TeleportableObject>();
            
            // Collider belongs to object in vicinity
            if (objectsInVicinity.FirstOrDefault(x => x.teleportableObject == tp) is { } nearbyTeleportable)
            {
                nearbyTeleportable.collidersInVicinity.Remove(other);
                
                // No more colliders in vicinity
                if(nearbyTeleportable.collidersInVicinity.Count == 0)
                    UnregisterTeleportable(nearbyTeleportable);
            }

            // Collider belongs to object clone in vicinity
            else if (other.GetComponentInParent<TeleportableObjectClone>() is { } tpClone)
            {
                if (pairCloneSystem.objectsInVicinity.FirstOrDefault(x => x.cloneTeleportableObject == tpClone) is { } nearbyPairTeleportable)
                    nearbyPairTeleportable.cloneColliderInPairVicinity.Remove(other);
            }
        }
        
        private NearbyTeleportable RegisterTeleportable(TeleportableObject teleportableObjectObject)
        {
            Debugger.LogInfo($"Registering {teleportableObjectObject} in {this}");
            
            // Create clone
            var clone = CreateClone(teleportableObjectObject, trackedPortal: portal);
            
            // Scale clone
            clone.transform.localScale *= 1 / portal.scaleRatio;
            
            // Register teleportable
            var nearbyTeleportable = new NearbyTeleportable(teleportableObjectObject, clone);
            nearbyTeleportable.cloneColliderInPairVicinity.AddRange(clone.transform.GetComponentsInChildren<Collider>());
            objectsInVicinity.Add(nearbyTeleportable);
            
            pairCloneSystem.vicinity.OnCloneIn?.Invoke(clone.gameObject);
            
            return nearbyTeleportable;
        }
        
        private void UnregisterTeleportable(NearbyTeleportable teleportable)
        { 
            Debugger.LogInfo($"Unregistering {teleportable} from {this}");
            
            // Destroy clone
            GameObjectUtility.DestroyNotify(teleportable.cloneTeleportableObject.gameObject);
            
            // Unregister teleportable
            objectsInVicinity.Remove(teleportable);
        }


        private void OnTeleportCallback(TeleportData teleportData)
        {
            // nt = "nearby teleportable"
            
            var nt = objectsInVicinity.FirstOrDefault(x => x.teleportableObject == teleportData.teleportableObject && portal == teleportData.inPortal);
            if(nt == null)
                return;
            
            // Scale clone
            nt.cloneTeleportableObject.transform.localScale *= teleportData.inPortal.scaleRatio;

            var swapData = new SwapData(nt.teleportableObject, nt.cloneTeleportableObject);
            
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
        
        
        private TeleportableObjectClone CreateClone(TeleportableObject original, Portal trackedPortal)
        {
            // Clone original
            var clone = GameObjectUtility.InstantiateNotify(original.gameObject);
            clone.transform.name = clone.transform.name.Replace("(Clone)", "");

            IEnumerable<Type> defaultPreservedComponents = new [] { typeof(Rigidbody), typeof(Collider), typeof(MeshRenderer), 
                                                                    typeof(MeshFilter), typeof(Transform), typeof(PhysicsObject) };
            
            // Strip joints immediately so it won't mess with physics
            foreach (var cloneJoint in clone.GetComponentsInChildren<TeleportableJoint>().Cast<Component>().Concat(clone.GetComponentsInChildren<Joint>()))
                GameObjectUtility.DestroyComponentImmediateNotify(cloneJoint);
            
            // Iterate children nodes
            foreach (var cloneNode in clone.GetComponentsInChildren<Component>().GroupBy(x => x.transform))
            {
                var preservedComponents = defaultPreservedComponents;
                
                // If node has TeleportableBehaviour
                if (cloneNode.OfType<ITeleportableBehaviour>().FirstOrDefault() is { } tb)
                {
                    tb.OnCreateClone(cloneNode.Key.gameObject, out var extraPreservedComponents);
                    preservedComponents = preservedComponents.Concat(extraPreservedComponents);
                    cloneNode.Key.gameObject.AddComponent<TeleportableBehaviourClone>();
                }

                // Remove components which are not in preservedComponents
                foreach (var component in cloneNode.ToArray())
                {
                    var componentType = component.GetType();
                    if (!preservedComponents.Any(preserveComp => preserveComp == componentType || componentType.IsSubclassOf(preserveComp)))
                        GameObjectUtility.DestroyComponentNotify(component);
                }
                
                // Append "(Clone)" to name
                cloneNode.Key.name += " (Clone)";
            }
            
            // PhysicsObject / Transform
            if (original.TryGetComponent<PhysicsObject>(out _)) clone.AddComponentNotify<PhysicsObjectClone>();
            else                                                clone.AddComponentNotify<TransformClone>();

            var teleportableClone = clone.AddComponentNotify<TeleportableObjectClone>();
            teleportableClone.Track(original, trackedPortal);
            
            Debugger.LogInfo($"{original.transform} was cloned.");

            return teleportableClone;
        }
    }
    
}