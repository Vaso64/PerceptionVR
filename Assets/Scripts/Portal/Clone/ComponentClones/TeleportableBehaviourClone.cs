using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portals
{
    public class TeleportableBehaviourClone : TrackedCloneBase<Transform>
    {
        private bool hasBehaviour;
        private ITeleportableBehaviour currenTeleportableBehaviourTarget;

        private void Awake()
        {
            // Transfer behaviour from target to self
            base.OnEnterPortal += () =>
            {
                hasBehaviour = true;
                currenTeleportableBehaviourTarget.TransferBehaviour(currenTeleportableBehaviourTarget.gameObject, gameObject);
                Debugger.LogInfo($"Transferred behaviour {currenTeleportableBehaviourTarget.GetType()} from  {currenTeleportableBehaviourTarget.gameObject} to {gameObject}");
            };
            
            // Transfer behaviour from self to target
            base.OnExitPortal += () =>
            {
                hasBehaviour = false;
                currenTeleportableBehaviourTarget.TransferBehaviour(gameObject, currenTeleportableBehaviourTarget.gameObject);
                Debugger.LogInfo($"Transferred behaviour {currenTeleportableBehaviourTarget.GetType()} from {gameObject} to {currenTeleportableBehaviourTarget.gameObject}");
            };
        }

        // Swap behaviour on main teleport
        private void Start() => GetComponentInParent<TeleportableObjectClone>().OnEnterPortal += SwapBehaviour;

        public void Track(ITeleportableBehaviour target, Portal throughPortal)
        {
            this.currenTeleportableBehaviourTarget = target;
            base.Track(target.transform, throughPortal);
        }

        private void SwapBehaviour()
        {
            if (hasBehaviour)
            {
                currenTeleportableBehaviourTarget.TransferBehaviour(gameObject, currenTeleportableBehaviourTarget.gameObject);
                Debugger.LogInfo($"Transferred behaviour {currenTeleportableBehaviourTarget.GetType()} from {gameObject} to {currenTeleportableBehaviourTarget.gameObject}");
            }
            else
            {
                currenTeleportableBehaviourTarget.TransferBehaviour(currenTeleportableBehaviourTarget.gameObject, gameObject);
                Debugger.LogInfo($"Transferred behaviour {currenTeleportableBehaviourTarget.GetType()} from  {currenTeleportableBehaviourTarget.gameObject} to {gameObject}");
            }
            hasBehaviour = !hasBehaviour;
        }
    }
}