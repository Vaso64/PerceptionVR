using System;
using MoreLinq.Extensions;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Global;
using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public class PortalCloneSystem : PortalCollisionFilteringSystem
    {
        private readonly List<NearbyObject> vicinity = new();

        private PortalCloneSystem pairCloneSystem;

        protected override void Start()
        {
            base.Start();
            pairCloneSystem = portal.portalPair.transform.GetComponentInChildren<PortalCloneSystem>();
            portal.OnTeleport += OnTeleport;
            OnEnteredVicinity += OnVicinityEnter;
            OnExitedVicinity += OnVicinityExit;
        }
        
        private void OnVicinityEnter(Collider other)
        {
            // If its clone
            var nearbyCloneObject = pairCloneSystem.vicinity.FirstOrDefault(x => x.associatedCloneColliders.Contains(other));
            if (nearbyCloneObject != null)
            {
                Debugger.LogInfo($"Clone {other} entered {this} vicinity");
                nearbyCloneObject.cloneCollidersInVicinity.Add(other);
                return;
            }
            
            // If its original
            Debugger.LogInfo($"{other} entered {this} vicinity");
            var nearbyObject = vicinity.FirstOrDefault(x => x.associatedColliders.Contains(other));
            if (nearbyObject == null)
            {
                var teleportable = other.GetComponentInParent<ITeleportable>();
                if (teleportable == null)
                {
                    Debugger.LogError($"{other} entered {this} vicinity but it is not teleportable");
                    return;
                }

                nearbyObject = RegisterTeleportable(teleportable);
            }
            nearbyObject.collidersInVicinity.Add(other);
        }
    
        private void OnVicinityExit(Collider other)
        {
            // If its clone
            var nearbyCloneObject = pairCloneSystem.vicinity.FirstOrDefault(x => x.associatedCloneColliders.Contains(other));
            if (nearbyCloneObject != null)
            {
                Debugger.LogInfo($"Clone {other} exited {this} vicinity");
                nearbyCloneObject.cloneCollidersInVicinity.Remove(other);
                return;
            }
            
            // If its original
            Debugger.LogInfo($"{other} exited {this} vicinity");
            var nearbyObject = vicinity.FirstOrDefault(x => x.associatedColliders.Contains(other));
            if (nearbyObject == null)
            {
                Debugger.LogError($"{other} exited vicinity but is not registered in {this}.");
                return;
            }
            nearbyObject.collidersInVicinity.Remove(other);
            if (nearbyObject.collidersInVicinity.Count == 0)
                UnregisterTeleportable(nearbyObject);
        }
        
        private NearbyObject RegisterTeleportable(ITeleportable teleportable)
        {
            var nearbyObject = new NearbyObject(teleportable, TeleportableClone.CreateClone(teleportable));
            nearbyObject.clone.Track(teleportable, portal);
            // TODO register clone to pair clone system
            nearbyObject.associatedCloneColliders.ForEach(x => pairCloneSystem.cloneGroup.Add(x));
            nearbyObject.cloneCollidersInVicinity.AddRange(nearbyObject.associatedCloneColliders);
            vicinity.Add(nearbyObject);
            Debugger.LogInfo($"Registered {teleportable} in {this}");
            return nearbyObject;
        }
        
        private void UnregisterTeleportable(NearbyObject nearbyObject)
        {
            vicinity.Remove(nearbyObject);
            GameObjectUtility.DestroyNotify(nearbyObject.clone.gameObject);
            Debugger.LogInfo($"Unregistered {nearbyObject.teleportable} from {this}");
        }

        private void OnTeleport(ITeleportable teleportable)
        {
            // Get NearbyObject
            var nearbyObject = vicinity.First(x => x.teleportable == teleportable);
            
            // Swap vicinity colliders
            nearbyObject.collidersInVicinity = nearbyObject.pairLookup
                .Where(x => nearbyObject.cloneCollidersInVicinity.Contains(x.clone))
                .Select(x => x.original).ToList();
            
            nearbyObject.cloneCollidersInVicinity = nearbyObject.pairLookup
                .Where(x => nearbyObject.collidersInVicinity.Contains(x.original))
                .Select(x => x.clone).ToList();


            // Exchange nearbyObject with pair's PortalCloneSystem
            vicinity.Remove(nearbyObject);
            pairCloneSystem.vicinity.Add(nearbyObject);
            
            base.OnTeleport(nearbyObject);
        }
    }
    
}