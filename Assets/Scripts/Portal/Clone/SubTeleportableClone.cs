using UnityEngine;

namespace PerceptionVR.Portal
{
    public class SubTeleportableClone : CloneBase, ISubTeleportable
    {
        public void TransferBehaviour(ISubTeleportable from, ISubTeleportable to) => targetSubTeleportable.TransferBehaviour(from, to);

        private ISubTeleportable targetSubTeleportable;
        
        private bool hasBehaviour = false;

        public void SwapBehaviour()
        {
            if (hasBehaviour) TransferBehaviour(this, targetSubTeleportable);
            else              TransferBehaviour(targetSubTeleportable, this);
            hasBehaviour = !hasBehaviour;
        }
        
        private void OnDestroy()
        {
            if (hasBehaviour)
            {
                Debug.LogWarning($"Clone {transform.name} has behaviour, but it's being destroyed (something is not right). Swapping...");
                //TransferBehaviour(this, targetSubTeleportable);
            }
        }

        // Start tracking coroutine
        public void Track(ISubTeleportable subTeleportable, IPortal portal)
        {
            targetSubTeleportable = subTeleportable;
            
            base.Track(targetSubTeleportable, portal);

            base.OnEnterPortal += () =>
            {
                hasBehaviour = true;
                targetSubTeleportable.TransferBehaviour(targetSubTeleportable, this);
            };
            base.OnExitPortal += () =>
            {
                hasBehaviour = false;
                targetSubTeleportable.TransferBehaviour(this, targetSubTeleportable);
            };
        }
    }
}