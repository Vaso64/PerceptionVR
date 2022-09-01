using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace PerceptionVR.Global
{
    public class ColliderGroup : ObservableCollection<Collider>
    {
        public enum FilterMode
        {
            Include, // Only collide with specified groups
            Exclude  // Collide with all groups except specified groups
        }
        
        public static ColliderGroup everything = default;

        private readonly List<Collider> ignoredColliders = default;
        private FilterMode filterMode;
        

        public void SetFilter(FilterMode mode, ColliderGroup group) => SetFilter(mode, new[] { group });
        
        public void SetFilter(FilterMode mode, ColliderGroup[] groups)
        {
            switch (mode)
            {
                case FilterMode.Exclude:
                    // Ignore all collider from all groups
                    IgnoreColliders(groups.SelectMany(x => x).Distinct());

                    // Register CollectionChanged callback to each group
                    foreach (var group in groups)
                    {
                        group.CollectionChanged += (sender, args) =>
                        {
                            IgnoreColliders(args.NewItems.Cast<Collider>(), true);
                            IgnoreColliders(args.OldItems.Cast<Collider>(), false);
                        };
                    }
                    
                    break;
                    
                case FilterMode.Include:
                    // Take everything except the groups
                    var combinedGroups = groups.SelectMany(x => x).Distinct();
                    IgnoreColliders(everything.Where(x => !combinedGroups.Contains(x)));
                    
                    // Register CollectionChanged callback to each group
                    foreach (var group in groups)
                    {
                        group.CollectionChanged += (sender, args) =>
                        {
                            IgnoreColliders(args.NewItems.Cast<Collider>(), false);
                            IgnoreColliders(args.OldItems.Cast<Collider>(), true);
                        };
                    }
                    
                    // And ignore any future colliders
                    everything.CollectionChanged += (sender, args) => IgnoreColliders(args.NewItems.Cast<Collider>());

                    break;
            }
        }
        

        public new void Add(Collider item)
        {
            ignoredColliders.ForEach(x => Physics.IgnoreCollision(item, x, true));
            base.Add(item);
        }
        
        public new bool Remove(Collider item)
        {
            ignoredColliders.ForEach(x => Physics.IgnoreCollision(item, x, false));
            return base.Remove(item);
        }

        public new void Clear()
        {
            foreach (var collider in this)
                Remove(collider);
        }

        
        private void IgnoreColliders(IEnumerable<Collider> colliders, bool ignore = true)
        {
            //                                 Maybe dont check when un-ignoring???
            foreach (var collider in colliders.Where(x => !this.Contains(x)))
            {
                if (ignore) ignoredColliders.Add(collider);
                else        ignoredColliders.Remove(collider);
                
                foreach (var thisCollider in this)
                    Physics.IgnoreCollision(collider, thisCollider, ignore);
            }
        }
    }
}