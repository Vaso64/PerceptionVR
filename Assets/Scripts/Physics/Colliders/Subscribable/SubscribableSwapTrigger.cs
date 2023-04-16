using System.Collections.Generic;
using PerceptionVR.Portals;
using PerceptionVR.Debug;
using PerceptionVR.Extensions;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class SubscribableSwapTrigger : SubscribableTrigger
    {
        private readonly List<Collider> shouldSwapIn = new();
        private readonly List<Collider> shouldSwapOut = new();
        private readonly Dictionary<ISwappable, List<Collider>> swappableCollidersInside = new();

        protected override void Awake()
        {
            base.Awake();

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
            // Try confirm swap in
            if (!shouldSwapIn.Remove(other))   
            {
                // Register swappable   
                TryRegisterSwappable(other);
                    
                // Do standard OnTriggerEnter
                base.OnTriggerEnter(other);    
            }    
                
        }
        
        protected override void OnTriggerExit(Collider other)
        {
            // Try confirm swap out
            if (!shouldSwapOut.Remove(other)) 
            {
                // Unregister swappable
                TryUnregisterSwappable(other);
                
                // Do standard OnTriggerExit
                base.OnTriggerExit(other);     
            }
                
        }
        
        private void PerformSwap(SwapData swapData)
        {
            SwapUtility.PerformSwap(collidersInside, swapData.colliderSwaps, out var appliedSwaps);
            foreach (var applicableSwap in appliedSwaps)
            {
                // Note:
                // I already have ISwappable in swapData.
                // If slow => Make "RegisterSwappable" without GetComponentInParent
                TryUnregisterSwappable(applicableSwap.removed);
                TryRegisterSwappable(applicableSwap.added);
                StartCoroutine(AddLastFrame(applicableSwap.added));
                StartCoroutine(RemoveLastFrame(applicableSwap.removed));
                shouldSwapOut.Add(applicableSwap.removed);
                shouldSwapIn.Add(applicableSwap.added);
            }
            ProcessQueue();
        }

        private void TryRegisterSwappable(Collider other)
        {
            if (other.TryGetComponentInParent(out ISwappable swappable))
            {
                if(swappableCollidersInside.TryGetValue(swappable, out var colliders))
                    colliders.Add(other);
                else
                {
                    swappableCollidersInside.Add(swappable, new List<Collider> {other});
                    swappable.OnSwap += PerformSwap;
                }
            }
        }
        
        private void TryUnregisterSwappable(Collider other)
        {
            if (other.TryGetComponentInParent(out ISwappable swappable) && swappableCollidersInside.TryGetValue(swappable, out var colliders))
            {
                colliders.Remove(other);
                if (colliders.Count == 0)
                {
                    swappableCollidersInside.Remove(swappable);
                    swappable.OnSwap -= PerformSwap;
                }
            }
        }
    }
}