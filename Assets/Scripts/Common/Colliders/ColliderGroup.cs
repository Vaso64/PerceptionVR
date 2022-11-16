using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using PerceptionVR.Global;
using UnityEngine;
using PerceptionVR.Common.Generic;
using PerceptionVR.Debug;

namespace PerceptionVR.Common
{
    public class ColliderGroup : ObservableCollection<Collider>
    {
        public enum FilterMode { Include, Exclude }
        
        private string debugName;

        public ColliderGroup(string debugName)
        {
            this.debugName = debugName;
            if (debugName != "")
            {
                OnAdded   += (colliders) => colliders.ForEach(c => Debugger.LogInfo($"{c} added to {debugName}"));
                OnRemoved += (colliders) => colliders.ForEach(c => Debugger.LogInfo($"{c} removed from {debugName}"));
            }
            
            // Remove destroyed colliders
            ComponentTracking.allColliders.OnRemoved += (colliders) => RemoveRange(colliders);
            
            // Un-ignore added colliders with rest of this group
            this.OnAdded += (colliders) => CollisionMatrix.SetCollisions(this, colliders, true);
        }

        public void SetFilter(FilterMode mode, IEnumerable<ColliderGroup> filterGroups)
        {
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
                    CollisionMatrix.SetCollisions(this, ComponentTracking.allColliders.Where(x => filterGroups.SelectMany(g => g).Distinct().Contains(x)), false);
                    
                    // Ignore all future colliders
                    ComponentTracking.allColliders.OnAdded += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                    
                    // Ignore future added colliders with colliders not in the filterGroups
                    this.OnAdded += colliders => CollisionMatrix.SetCollisions(colliders, ComponentTracking.allColliders.Except(filterGroups.SelectMany(g => g)), false);
                    
                    // Un-ignore and ignore colliders added / removed from the filterGroups
                    foreach (var group in filterGroups)
                    {
                        group.OnAdded   += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
                        group.OnRemoved += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                    }
                    break;
            }
        }
    }
}