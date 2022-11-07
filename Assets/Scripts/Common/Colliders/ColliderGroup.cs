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

        private static ComponentTracker<Collider> _allColliders = new ();

        public ColliderGroup(string debugName)
        {
            if (debugName != "")
            {
                OnAdded   += (colliders) => colliders.ForEach(c => Debugger.LogInfo($"{c} added to {debugName}"));
                OnRemoved += (colliders) => colliders.ForEach(c => Debugger.LogInfo($"{c} removed from {debugName}"));
            }
        }

        public void SetFilter(FilterMode mode, IEnumerable<ColliderGroup> groups)
        {
            switch (mode)
            {   
                case FilterMode.Exclude:
                    foreach (var group in groups)
                    {
                        CollisionMatrix.SetCollisions(this, group, true);
                        group.OnAdded   += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                        group.OnRemoved += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
                    }
                    break;
                case FilterMode.Include:
                    CollisionMatrix.SetCollisions(this, _allColliders, false);
                    foreach (var group in groups)
                    {
                        CollisionMatrix.SetCollisions(this, group, true);
                        group.OnAdded   += colliders => CollisionMatrix.SetCollisions(this, colliders, true);
                        group.OnRemoved += colliders => CollisionMatrix.SetCollisions(this, colliders, false);
                    }
                    break;
            }
        }
    }
}