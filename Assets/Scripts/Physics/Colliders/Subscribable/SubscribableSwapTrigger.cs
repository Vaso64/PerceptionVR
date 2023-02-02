using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Portals;
using PerceptionVR.Common.Generic;
using PerceptionVR.Common;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class SubscribableSwapTrigger : SubscribableTrigger
    {
        private readonly TemporaryCollection<Collider> swappedInThisFrame  = new (FrameType.Fixed, 1);
        private readonly TemporaryCollection<Collider> swappedOutThisFrame = new (FrameType.Fixed, 1);

        protected override void Awake()
        {
            base.Awake();
            GlobalEvents.OnSwap += (swapData) =>
            {
                var applicableSwaps = SwapUtility.GetApplicableSwaps(collidersInside, swapData.colliderSwaps);
                foreach (var applicableSwap in applicableSwaps)
                {
                    swappedInThisFrame.Add(applicableSwap.toAdd);
                    swappedOutThisFrame.Add(applicableSwap.toRemove);
                }
            };
            
            OnBeforeProcessQueue += () =>
            {
                // Colliders that swapped in but didn't triggered the OnTriggerEnter will be added to the exit queue
                swappedInThisFrame.Where(x => !collidersInside.Contains(x)).ForEach(x => onTriggerExitQueue.Enqueue(x));
                // Colliders that swapped out but didn't triggered the OnTriggerExit will be added to the enter queue
                swappedOutThisFrame.Where(x => collidersInside.Contains(x)).ForEach(x => onTriggerEnterQueue.Enqueue(x));
            };
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            //Debugger.LogInfo($"OnTriggerEnter {other} in {this}");
            
            // Confirm swap in
            if(swappedInThisFrame.Contains(other))
                collidersInside.Add(other);
            
            // Standard OnTriggerEnter
            else
                base.OnTriggerEnter(other);
        }
        
        protected override void OnTriggerExit(Collider other)
        {
            //Debugger.LogInfo($"OnTriggerExit {other} from {this}");
            
            // Confirm swap out
            if(swappedOutThisFrame.Contains(other))
                collidersInside.Remove(other);
            
            // Standard OnTriggerExit
            else
                base.OnTriggerExit(other);
        }
    }
}