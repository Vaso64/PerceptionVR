using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Global;
using UnityEngine;
using PerceptionVR.Common.Generic;
using PerceptionVR.Debug;
using PerceptionVR.Portal;

namespace PerceptionVR.Physics
{
    [System.Serializable] 
    public class ColliderGroup : ObservableCollection<Collider>
    {
        public enum FilterMode { Include, Exclude }

        [HideInInspector] public string debugName = "";

        public void SetFilter(FilterMode mode, IEnumerable<ColliderGroup> filterGroups)
        {
            OnAdded += (colliders) => colliders.ForEach(c => Debugger.LogInfo($"{c} added to {debugName}"));
            OnRemoved += (colliders) => colliders.ForEach(c => Debugger.LogInfo($"{c} removed from {debugName}"));

            // Remove destroyed colliders
            ComponentTracking.allColliders.OnRemoved += RemoveRange;
            
            // Swap colliders on swap event
            GlobalEvents.OnSwap += (swapData) => SwapUtility.PerformSwap(this, swapData.colliderSwaps);
            
            // Un-ignore added colliders with rest of this group
            OnAdded += (colliders) => CollisionMatrix.SetCollisions(this, colliders, true);
            
            switch (mode)
            {
                case FilterMode.Exclude:
                    // Ignore with all colliders in the groups
                    CollisionMatrix.SetCollisions(this, filterGroups.SelectMany(g => g).Distinct(), false);
                    
                    // Ignore and un-ignore colliders added / removed from the filterGroups
                    foreach (var group in filterGroups)
                    {
                        group.OnAdded   += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                        group.OnRemoved += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
                    }
                    break;
                
                case FilterMode.Include:
                    // Ignore with colliders not in filterGroups
                    CollisionMatrix.SetCollisions(this, ComponentTracking.allColliders.Except(filterGroups.SelectMany(g => g).Union(this)), false);
                    
                    // Ignore all future colliders
                    ComponentTracking.allColliders.OnAdded += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                    
                    // Ignore future added colliders with colliders not in the filterGroups
                    OnAdded += colliders => CollisionMatrix.SetCollisions(colliders, ComponentTracking.allColliders.Except(filterGroups.SelectMany(g => g).Union(this)), false);
                    OnRemoved += colliders => CollisionMatrix.SetCollisions(colliders, ComponentTracking.allColliders, true);
                    
                    // Un-ignore and ignore colliders added / removed from the filterGroups
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
            base.AddRange(colliders.Where(x => !Contains(x)));
        }

        public new void Add(Collider collider)
        {
            if (!Contains(collider))
                base.Add(collider);
        }
    }
}