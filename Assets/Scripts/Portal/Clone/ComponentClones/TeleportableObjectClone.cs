using System;
using System.Linq;
using UnityEngine;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using PerceptionVR.Physics;


namespace PerceptionVR.Portals
{
    public class TeleportableObjectClone : TrackedCloneBase<TeleportableObject>
    {
        public event Action<TeleportData> OnTeleport; 

        private PhysicsObjectClone physicsObjectClone;
        private LocalTransformClone[] transformClones;
        private TeleportableBehaviourClone[] teleportableBehaviourClones;

        private void Awake()
        { 
            base.OnEnterPortal += OnEnterPortalCallback;
            physicsObjectClone = GetComponent<PhysicsObjectClone>();
            transformClones = GetComponentsInChildren<LocalTransformClone>();
            teleportableBehaviourClones = GetComponentsInChildren<TeleportableBehaviourClone>();
        } 
        
        public override void Track(TeleportableObject targetTeleportable, Portal throughPortal)
        {
            // Track PhysicsObject
            physicsObjectClone.Track(targetTeleportable.GetComponent<PhysicsObject>(), throughPortal);
            
            // Track ITeleportableBehaviours
            foreach (var tbPair in teleportableBehaviourClones.Zip(targetTeleportable.GetComponentsInChildren<ITeleportableBehaviour>(), (clone, target) => (clone, target)))
                tbPair.clone.Track(tbPair.target, throughPortal);
            
            // Track Transforms
            foreach (var transformPairs in transformClones.Zip(targetTeleportable.GetComponentsInChildren<Transform>().Where(t => t != targetTeleportable.transform), (clone, target) => (clone, target)))
                transformPairs.clone.Track(transformPairs.target, throughPortal);

            // Un/subscribe to target teleport event
            if(targetSet) currentTarget.OnTeleport -= OnTargetTeleportCallback;
            targetTeleportable.OnTeleport += OnTargetTeleportCallback;
            
            // Track TeleportableObject
            base.Track(targetTeleportable, throughPortal);
        }
        
        private void OnEnterPortalCallback()
        {
            // Don't teleport if teleportation is controlled manually
            if(currentTarget.manualTeleport)
                return;
            
            Debugger.LogInfo($"{currentTarget} ENTERED PORTAL!");
            currentThroughPortal.Teleport(currentTarget);
        }

        private void OnTargetTeleportCallback(TeleportData data)
        {
            if (data.inPortal == currentThroughPortal)
            {
                OnTeleport?.Invoke(data);
                Track(data.teleportableObject, data.outPortal);
            }
            else
            {
                Debugger.LogWarning("Tracked object was teleported through different portal. TODO: Destroy self / track swapped clone instead.");
                GameObjectUtility.DestroyNotify(gameObject);
            }
        } 

        private void OnDestroy() => currentTarget.OnTeleport -= OnTargetTeleportCallback;
    }
}