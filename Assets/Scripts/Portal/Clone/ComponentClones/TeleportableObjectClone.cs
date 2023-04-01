using System;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Common;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using PerceptionVR.Physics;


namespace PerceptionVR.Portals
{
    public class TeleportableObjectClone : TrackedCloneBase<TeleportableObject>
    {
        

        private void Awake() => base.OnEnterPortal += OnEnterPortalCallback;
        
        public override void Track(TeleportableObject target, Portal throughPortal)
        {
            // Position clone
            transform.SetPose(throughPortal.PairPose(target.transform.GetPose()));
            
            // Track physics object
            if(target.TryGetComponent(out PhysicsObject physicsObject) && this.TryGetComponent(out PhysicsObjectClone physicsObjectClone))
                physicsObjectClone.Track(physicsObject, throughPortal);
            else
                GetComponent<TransformClone>().Track(target.transform, throughPortal);

            // Track teleportable behaviour
            foreach (var tbPair in ComponentUtility.CreateComponentTuple<ITeleportableBehaviour, TeleportableBehaviourClone>(target.transform, transform))
                tbPair.clone.Track(tbPair.original, throughPortal);

            if(targetSet)
                currentTarget.OnTeleport -= OnTargetTeleportCallback;
            target.OnTeleport += OnTargetTeleportCallback;
            base.Track(target, throughPortal);
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
            if(data.inPortal == currentThroughPortal)
                Track(data.teleportableObject, data.outPortal);
            else
            {
                Debugger.LogWarning("Tracked object was teleported through different portal. TODO: Destroy self / track swapped clone instead.");
                GameObjectUtility.DestroyNotify(gameObject);
            }
        } 

        private void OnDestroy() => currentTarget.OnTeleport -= OnTargetTeleportCallback;
    }
}