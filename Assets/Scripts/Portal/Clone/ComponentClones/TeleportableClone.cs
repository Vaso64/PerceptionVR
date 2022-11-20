using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Common;
using PerceptionVR.Global;
using System.Collections.Generic;
using System.Linq;
using System;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public class TeleportableClone : TrackedCloneBase<Transform>, ITeleportable
    {

        private ITeleportable targetTeleportable;

        private void Awake()
        {
            base.OnEnterPortal += OnEnterPortalCallback;
        }

        public void Track(ITeleportable target, IPortal throughPortal)
        {
            // Position clone
            transform.SetPose(throughPortal.PairPose(target.transform.GetPose()));
            
            // Track rigidbody
            foreach (var movePair in ComponentUtility.CreateComponentTuple<Transform, MovementClone>(target.transform, transform))
                movePair.clone.Track(movePair.original, throughPortal);

            // Track teleportable behaviour
            foreach (var tbPair in ComponentUtility.CreateComponentTuple<ITeleportableBehaviour, TeleportableBehaviourClone>(target.transform, transform))
                tbPair.clone.Track(tbPair.original, throughPortal);

            this.targetTeleportable = target;
            base.Track(target.transform, throughPortal);
        }
        
        private void OnEnterPortalCallback()
        {
            Debugger.LogInfo($"{targetTeleportable} ENTERED PORTAL!");
            var teleportData = new TeleportData(targetTeleportable, this, throughPortal);
            throughPortal.Teleport(teleportData);
            Track(targetTeleportable, throughPortal.portalPair);
        }
    }
}