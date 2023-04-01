using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Global;
using UnityEngine;
using PerceptionVR.Common.Generic;
using PerceptionVR.Debug;
using PerceptionVR.Portals;

namespace PerceptionVR.Physics
{
    [System.Serializable] 
    public class ColliderGroup : ObservableCollection<Collider>
    {
        public enum FilterMode { Include, Exclude }

        [HideInInspector] public string debugName = "";

        public void SetFilter(FilterMode mode, IEnumerable<ColliderGroup> filterGroups)
        {
            //OnAdded += colliders => colliders.ForEach(c => Debugger.LogInfo($"{c} added to {debugName}"));
            //OnRemoved += colliders => colliders.ForEach(c => Debugger.LogInfo($"{c} removed from {debugName}"));

            // Remove destroyed colliders   (Replaced by GetEnumerator override => collection clears itself when iterating)
            //ComponentTracking.allColliders.OnRemoved += RemoveRange;        
            
            // Swap colliders on swap event
            GlobalEvents.OnSwap += swapData => SwapUtility.PerformSwap(this, swapData.colliderSwaps);
            
            // Un-ignore added colliders with rest of this group
            OnAdded += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
            
            switch (mode)
            {
                case FilterMode.Exclude:
                    // Ignore with colliders in the filterGroups
                    CollisionMatrix.SetCollisions(this, filterGroups.SelectMany(g => g).Distinct(), false);
                    
                    // Ignore future colliders added to this group with filterGroups
                    OnAdded   += colliders => filterGroups.ForEach(x => CollisionMatrix.SetCollisions(x, colliders, false));
                    OnRemoved += colliders => filterGroups.ForEach(x => CollisionMatrix.SetCollisions(x, colliders, true));
                    
                    // Ignore future colliders added to the filterGroups
                    foreach (var group in filterGroups)
                    {
                        group.OnAdded   += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                        group.OnRemoved += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
                    }
                    break;
                
                // Expensive (don't use)
                case FilterMode.Include:
                    Debugger.LogWarning("FilterMode.Include is expensive and should not be used");
                    
                    // Ignore with all colliders not in filterGroups
                    CollisionMatrix.SetCollisions(this, ComponentTracking.allColliders.Except(filterGroups.SelectMany(g => g).Union(this)), false);
                    
                    // Ignore all future colliders
                    ComponentTracking.allColliders.OnAdded += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                    
                    // Un-ignore all future colliders added to this group
                    OnAdded += colliders => CollisionMatrix.SetCollisions(colliders, ComponentTracking.allColliders.Except(filterGroups.SelectMany(g => g).Union(this)), false);
                    OnRemoved += colliders => CollisionMatrix.SetCollisions(colliders, ComponentTracking.allColliders, true);
                    
                    // Un-ignore future colliders added to the filterGroups
                    foreach (var group in filterGroups)
                    {
                        group.OnAdded   += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
                        group.OnRemoved += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                    }
                    break;
            }
        }
        
        // Duplicate protection
        public new void AddRange(IEnumerable<Collider> colliders)
        {
            base.AddRange(colliders.Where(x => !Contains(x)).ToList());
        }

        public new void Add(Collider collider)
        {
            if (!Contains(collider))
                base.Add(collider);
        }
        
        // Auto remove null items before enumerating
        public override IEnumerator<Collider> GetEnumerator()
        {
            collection.RemoveAll(x => x == null);
            return base.GetEnumerator();
        }
    }
}