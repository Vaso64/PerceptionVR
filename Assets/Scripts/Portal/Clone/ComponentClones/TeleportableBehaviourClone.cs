using System;
using PerceptionVR.Extensions;
using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public class TeleportableBehaviourClone : TrackedCloneBase<Transform>
    {
        private bool hasBehaviour;
        private ITeleportableBehaviour targetTeleportableBehaviour;

        private void Awake()
        {
            // Transfer behaviour from target to self
            base.OnEnterPortal += () =>
            {
                hasBehaviour = true;
                targetTeleportableBehaviour.TransferBehaviour(targetTeleportableBehaviour.gameObject, gameObject);
                Debugger.LogInfo($"Transferred behaviour {targetTeleportableBehaviour.GetType()} from  {targetTeleportableBehaviour.gameObject} to {gameObject}");
            };
            
            // Transfer behaviour from self to target
            base.OnExitPortal += () =>
            {
                hasBehaviour = false;
                targetTeleportableBehaviour.TransferBehaviour(gameObject, targetTeleportableBehaviour.gameObject);
                Debugger.LogInfo($"Transferred behaviour {targetTeleportableBehaviour.GetType()} from {gameObject} to {targetTeleportableBehaviour.gameObject}");
            };
        }

        // Swap behaviour on main teleport
        private void Start() => GetComponentInParent<TeleportableClone>().OnTeleport += SwapBehaviour;

        public void Track(ITeleportableBehaviour target, IPortal throughPortal)
        {
            this.targetTeleportableBehaviour = target;
            base.Track(target.transform, throughPortal);
        }

        private void SwapBehaviour()
        {
            if (hasBehaviour)
            {
                targetTeleportableBehaviour.TransferBehaviour(gameObject, targetTeleportableBehaviour.gameObject);
                Debugger.LogInfo($"Transferred behaviour {targetTeleportableBehaviour.GetType()} from {gameObject} to {targetTeleportableBehaviour.gameObject}");
            }
            else
            {
                targetTeleportableBehaviour.TransferBehaviour(targetTeleportableBehaviour.gameObject, gameObject);
                Debugger.LogInfo($"Transferred behaviour {targetTeleportableBehaviour.GetType()} from  {targetTeleportableBehaviour.gameObject} to {gameObject}");
            }
            hasBehaviour = !hasBehaviour;
        }
    }
}