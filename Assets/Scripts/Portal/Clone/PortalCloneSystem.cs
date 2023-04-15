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
        [Serializable]
        public class NearbyTeleportable
        {
            public readonly TeleportableObject teleportableObject;
            public readonly TeleportableObjectClone cloneTeleportableObject;
            public List<Collider> collidersInVicinity;
            public List<Collider> cloneColliderInPairVicinity;
        
            public NearbyTeleportable(TeleportableObject teleportableObject, TeleportableObjectClone teleportableObjectClone)
            {
                this.teleportableObject = teleportableObject;
                this.cloneTeleportableObject = teleportableObjectClone;
                this.collidersInVicinity = new List<Collider>();
                this.cloneColliderInPairVicinity = new List<Collider>();
            }
        }
        
        private readonly Dictionary<TeleportableObject, NearbyTeleportable> objectsInVicinity = new();
        [SerializeField] private List<NearbyTeleportable> serializedObjectsInVicinity = new();

        private Portal portal;
        private PortalVicinity vicinity;
        private PortalCloneSystem pairCloneSystem;
        
        private void Awake()
        {
            portal = GetComponent<Portal>();
            portal.OnPortalPairSet   += portalPair => pairCloneSystem = portalPair.transform.GetComponentInChildren<PortalCloneSystem>();
            portal.OnPortalPairUnset += () =>
            {
                objectsInVicinity.ToList().ForEach(x => UnregisterTeleportable(x.Value));
                pairCloneSystem = null;
            };
            portal.OnTeleport += OnTeleportCallback;
            //GlobalEvents.OnTeleport += OnTeleportCallback;
            
            vicinity = GetComponent<PortalVicinity>();
            vicinity.OnLargeToFront += OnVicinityEnter;
            vicinity.OnFrontToLarge += OnVicinityExit;
        }


        private void OnVicinityEnter(Collider other)
        {
            Debugger.LogInfo($"{other} entered vicinity of {this}");
            
            // Collider belongs to teleportable
            if (other.GetComponentInParent<TeleportableObject>() is { } tp)
            {
                if(objectsInVicinity.TryGetValue(tp, out var nearbyTp))
                    nearbyTp.collidersInVicinity.Add(other);
                
                // Teleportable not yet registered in vicinity
                else
                    RegisterTeleportable(tp).collidersInVicinity.Add(other);
            }
            
            // Collider belongs to teleportable clone
            else if (other.GetComponentInParent<TeleportableObjectClone>() is { } tpClone)
            {
                // Clone is registered in pair's vicinity
                if (pairCloneSystem.objectsInVicinity.TryGetValue(tpClone.currentTarget, out var nearbyPairTp))
                    nearbyPairTp.cloneColliderInPairVicinity.Add(other);
            }
            
            // Unknown collider
            else 
                Debugger.LogWarning($"Collider {other} is not neither teleportable or teleportable clone");
        }
    
        private void OnVicinityExit(Collider other)
        {
            Debugger.LogInfo($"{other} exited vicinity of {this}");

            // Collider belongs to object in vicinity
            if (other.GetComponentInParent<TeleportableObject>() is { } tp && objectsInVicinity.TryGetValue(tp, out var nearbyTp))
            {
                nearbyTp.collidersInVicinity.Remove(other);

                    // No more colliders in vicinity
                if(nearbyTp.collidersInVicinity.Count == 0)
                    UnregisterTeleportable(nearbyTp);
            }

            // Collider belongs to object clone in vicinity
            else if (other.GetComponentInParent<TeleportableObjectClone>() is { } tpClone)
            {
                if (pairCloneSystem.objectsInVicinity.TryGetValue(tpClone.currentTarget, out var nearbyPairTp))
                    nearbyPairTp.cloneColliderInPairVicinity.Remove(other);
            }
        }
        
        private NearbyTeleportable RegisterTeleportable(TeleportableObject teleportableObject)
        {
            Debugger.LogInfo($"Registering {teleportableObject} in {this}");
            
            // Create clone
            var clone = CreateClone(teleportableObject);
            
            // Track clone
            clone.Track(teleportableObject, portal);
            
            // Scale clone
            clone.transform.localScale *= 1 / portal.scaleRatio;
            
            // Register teleportable
            var nt = new NearbyTeleportable(teleportableObject, clone);
            nt.cloneColliderInPairVicinity.AddRange(clone.transform.GetComponentsInChildren<Collider>());
            objectsInVicinity.Add(nt.teleportableObject, nt);
            serializedObjectsInVicinity.Add(nt);
            
            pairCloneSystem.vicinity.OnCloneIn?.Invoke(clone.gameObject);
            
            return nt;
        }
        
        private void UnregisterTeleportable(NearbyTeleportable teleportable)
        { 
            Debugger.LogInfo($"Unregistering {teleportable} from {this}");
            
            // Destroy clone
            GameObjectUtility.DestroyNotify(teleportable.cloneTeleportableObject.gameObject);
            
            // Unregister teleportable
            objectsInVicinity.Remove(teleportable.teleportableObject);
            serializedObjectsInVicinity.Remove(teleportable);
        }


        private void OnTeleportCallback(TeleportData teleportData)
        {
            // nt = "nearby teleportable"
            if(!objectsInVicinity.TryGetValue(teleportData.teleportableObject, out var nt))
                return;

            // Scale clone
            nt.cloneTeleportableObject.transform.localScale *= teleportData.inPortal.scaleRatio;

            var swapData = new SwapData(nt.teleportableObject, nt.cloneTeleportableObject);
            
            // Notify swap
            GlobalEvents.OnSwap?.Invoke(swapData);

            //Debugger.LogInfo($"Swapping teleportables of {swapData.teleportableSwap.Item1} and {swapData.teleportableSwap.Item2} in {this}");

            // Swap colliders
            SwapUtility.PerformSwap(nt.collidersInVicinity, swapData.colliderSwaps);
            SwapUtility.PerformSwap(nt.cloneColliderInPairVicinity, swapData.colliderSwaps);
            
            // Swap collider lists
            (nt.collidersInVicinity, nt.cloneColliderInPairVicinity) = (nt.cloneColliderInPairVicinity, nt.collidersInVicinity);

            // Transfer nt to pair
            objectsInVicinity.Remove(nt.teleportableObject);
            serializedObjectsInVicinity.Remove(nt);
            pairCloneSystem.objectsInVicinity.Add(nt.teleportableObject, nt);
            pairCloneSystem.serializedObjectsInVicinity.Add(nt);
        }
        
        
        private TeleportableObjectClone CreateClone(TeleportableObject original)
        {
            // Clone original
            var clone = GameObjectUtility.InstantiateNotify(original.gameObject);
            clone.transform.name = clone.transform.name.Replace("(Clone)", "");

            IEnumerable<Type> defaultPreservedComponents = new [] { typeof(Rigidbody),  typeof(Collider),  typeof(MeshRenderer), 
                                                                    typeof(MeshFilter), typeof(Transform), typeof(PhysicsObject) };
            
            // Strip joints immediately so it won't mess with physics
            foreach (var cloneJoint in clone.GetComponentsInChildren<TeleportableJoint>().Cast<Component>().Concat(clone.GetComponentsInChildren<Joint>()))
                GameObjectUtility.DestroyComponentImmediateNotify(cloneJoint);
            
            var cloneData = new CloneData(original.gameObject, clone, portal, portal.portalPair);
            
            // Iterate children nodes
            foreach (var cloneNode in clone.GetComponentsInChildren<Component>().GroupBy(x => x.transform))
            {
                var preservedComponents = defaultPreservedComponents;
                
                // If node has TeleportableBehaviour
                if (cloneNode.OfType<ITeleportableBehaviour>().FirstOrDefault() is { } tb)
                {
                    cloneData.clone = tb.gameObject;
                    tb.OnCreateClone(cloneData, out var extraPreservedComponents);
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
                
                // Add TransformClone
                if (cloneNode.Key != clone.transform)
                    cloneNode.Key.gameObject.AddComponent<LocalTransformClone>();
                
                // Append "(Clone)" to name
                cloneNode.Key.name += " (Clone)";
            }
            
            // PhysicsObject / Transform
            if (original.TryGetComponent<PhysicsObject>(out _)) clone.AddComponentNotify<PhysicsObjectClone>();
            else                                                clone.AddComponentNotify<LocalTransformClone>();

            var teleportableClone = clone.AddComponentNotify<TeleportableObjectClone>();

            Debugger.LogInfo($"{original.transform} was cloned.");

            return teleportableClone;
        }
    }
    
}