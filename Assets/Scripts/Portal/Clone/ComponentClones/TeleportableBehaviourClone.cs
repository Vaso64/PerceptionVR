using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portals
{
    public class TeleportableBehaviourClone : TrackedCloneBase<Transform>
    {
        private ITeleportableBehaviour currenTeleportableBehaviourTarget;
        private CloneData cloneData;

        private void Awake()
        {
            // Transfer behaviour from target to self
            base.OnEnterPortal += () =>
            {
                currenTeleportableBehaviourTarget.OnEnterPortal(cloneData);
                Debugger.LogInfo($"Transferred behaviour {currenTeleportableBehaviourTarget.GetType()} from  {currenTeleportableBehaviourTarget.gameObject} to {gameObject}");
            };
            
            // Transfer behaviour from self to target
            base.OnExitPortal += () =>
            {
                currenTeleportableBehaviourTarget.OnExitPortal(cloneData);
                Debugger.LogInfo($"Transferred behaviour {currenTeleportableBehaviourTarget.GetType()} from {gameObject} to {currenTeleportableBehaviourTarget.gameObject}");
            };
        }

        // Swap behaviour on main teleport
        private void Start() => GetComponentInParent<TeleportableObjectClone>().OnTeleport += SwapBehaviour;

        public void Track(ITeleportableBehaviour target, Portal throughPortal)
        {
            this.currenTeleportableBehaviourTarget = target;
            cloneData = new CloneData(target.gameObject, gameObject, throughPortal.portalPair, throughPortal);
            base.Track(target.transform, throughPortal);
        }

        private void SwapBehaviour(TeleportData _)
        {
            currenTeleportableBehaviourTarget.OnTeleport(cloneData);
            Debugger.LogInfo($"Swapped behaviour {currenTeleportableBehaviourTarget.GetType()} between {gameObject} and {currenTeleportableBehaviourTarget.gameObject}");
        }
    }
}