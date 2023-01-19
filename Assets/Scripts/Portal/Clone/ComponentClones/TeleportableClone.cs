using System;
using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Common;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public class TeleportableClone : TrackedCloneBase<Transform>, ITeleportable
    {
        public Action<TeleportData> OnTeleport { get; set; }
        public bool manualTeleport { get; set; }

        private ITeleportable targetTeleportable;

        private void Awake()
        {
            base.OnEnterPortal += OnEnterPortalCallback;
        }

        public void Track(ITeleportable target, Portal throughPortal)
        {
            // Position clone
            transform.SetPose(throughPortal.PairPose(target.transform.GetPose()));
            
            // Track rigidbody
            foreach (var movePair in ComponentUtility.CreateComponentTuple<Transform, MovementClone>(target.transform, transform))
                movePair.clone.Track(movePair.original, throughPortal);

            // Track teleportable behaviour
            foreach (var tbPair in ComponentUtility.CreateComponentTuple<ITeleportableBehaviour, TeleportableBehaviourClone>(target.transform, transform))
                tbPair.clone.Track(tbPair.original, throughPortal);

            if(targetTeleportable != null)
                targetTeleportable.OnTeleport -= OnTargetTeleportCallback; 
            targetTeleportable = target;
            targetTeleportable.OnTeleport += OnTargetTeleportCallback;
            base.Track(target.transform, throughPortal);
        }
        
        private void OnEnterPortalCallback()
        {
            // Don't teleport if teleportation is controlled manually
            if(targetTeleportable.manualTeleport)
                return;
            
            Debugger.LogInfo($"{targetTeleportable} ENTERED PORTAL!");
            throughPortal.Teleport(targetTeleportable);
        }
        
        private void OnTargetTeleportCallback(TeleportData data) => Track(data.teleportable, data.outPortal);

        private void OnDestroy() => targetTeleportable.OnTeleport -= OnTargetTeleportCallback;
    }
}