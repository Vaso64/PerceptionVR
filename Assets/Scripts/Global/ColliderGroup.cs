using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MoreLinq;
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
        
        private static ColliderGroup everything = new ColliderGroupEverything();

        private readonly List<Collider> ignoredColliders = new List<Collider>();
        private FilterMode filterMode;

        public void SetFilter(FilterMode mode, IEnumerable<ColliderGroup> groups)
        {
            var combinedGroups = groups.SelectMany(g => g).Distinct();
            switch (mode)
            {
                case FilterMode.Exclude:
                    // Ignore all collider from all groups
                    IgnoreColliders(combinedGroups);

                    // Register CollectionChanged callback to each group
                    foreach (var group in groups)
                        group.CollectionChanged += (sender, args) =>
                        {
                            if(args.NewItems != null) IgnoreColliders(args.NewItems.Cast<Collider>(), true);
                            if(args.OldItems != null) IgnoreColliders(args.OldItems.Cast<Collider>(), false);
                        };

                    break;
                    
                case FilterMode.Include:
                    // Take everything except the groups
                    IgnoreColliders(everything.Where(x => !combinedGroups.Contains(x)));
                    
                    // Register CollectionChanged callback to each group
                    foreach (var group in groups)
                        group.CollectionChanged += (sender, args) =>
                        {
                            if(args.NewItems != null) IgnoreColliders(args.NewItems.Cast<Collider>(), false);
                            if(args.OldItems != null) IgnoreColliders(args.OldItems.Cast<Collider>(), true);
                        };

                    // And ignore any future colliders
                    everything.CollectionChanged += (sender, args) =>
                    {
                        if (args.NewItems != null) IgnoreColliders(args.NewItems.Cast<Collider>());
                    };
                    break;
                
                default:
                    return;
            }
            
            everything.CollectionChanged += (sender, args) =>
            {
                if (args.OldItems != null)
                    foreach (var destroyedCollider in args.OldItems.Cast<Collider>())
                    {
                        ignoredColliders.Remove(destroyedCollider);
                        base.Items.Remove(destroyedCollider);
                    }
            };
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
            foreach (var collider in colliders)
            {
                if (ignore) ignoredColliders.Add(collider);
                else        ignoredColliders.Remove(collider);
                
                foreach (var thisCollider in this)
                    Physics.IgnoreCollision(collider, thisCollider, ignore);
            }
        }
    }
}