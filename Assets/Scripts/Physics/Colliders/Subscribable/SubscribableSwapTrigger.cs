using System.Collections.Generic;
using PerceptionVR.Portals;
using PerceptionVR.Debug;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class SubscribableSwapTrigger : SubscribableTrigger
    {
        private readonly List<Collider> shouldSwapIn = new();
        private readonly List<Collider> shouldSwapOut = new();

        protected override void Awake()
        {
            base.Awake();
            GlobalEvents.OnSwap += swapData =>
            {
                if(collidersInside.Count == 0)
                    return;
                var applicableSwaps = SwapUtility.GetApplicableSwaps(collidersInside, swapData.colliderSwaps);
                foreach (var applicableSwap in applicableSwaps)
                {
                    collidersInside.Remove(applicableSwap.toRemove);
                    collidersInside.Add(applicableSwap.toAdd);
                    shouldSwapOut.Add(applicableSwap.toRemove);
                    shouldSwapIn.Add(applicableSwap.toAdd);
                }
            };
            
            OnBeforeProcessQueue += () =>
            {
                if (shouldSwapIn.Count != 0)
                {
                    foreach (var didntSwappedIn in shouldSwapIn)
                    {
                        Debugger.LogWarning($"{didntSwappedIn} swapped in to {this} but it did not trigger OnTriggerEnter. " +
                                            "Assuming it left the trigger in a swap frame");
                        base.OnTriggerExit(didntSwappedIn);
                    }
                    shouldSwapIn.Clear();
                }

                if (shouldSwapOut.Count != 0)
                {
                    foreach (var didntSwappedOut in shouldSwapOut)
                    {
                        Debugger.LogWarning($"{didntSwappedOut} swapped out of {this} but it did not trigger OnTriggerExit. " +
                                            "Don't know what to do here...");
                        //base.OnTriggerEnter(didntSwappedOut);
                    }
                    shouldSwapOut.Clear();
                }

            };
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            if(!shouldSwapIn.Remove(other))    // Confirm swap in
                base.OnTriggerEnter(other);    // Or do standard OnTriggerEnter
        }
        
        protected override void OnTriggerExit(Collider other)
        {
            if(!shouldSwapOut.Remove(other))   // Confirm swap out
                base.OnTriggerExit(other);     // Or do standard OnTriggerExit
        }
    }
}