using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace PerceptionVR.Global
{
    public record ColliderGroup
    {
        public readonly ObservableCollection<Collider> colliders;
        
        private ICollection<ColliderGroup> ignoredGroups;

        public ColliderGroup()
        {
            colliders = new ObservableCollection<Collider>();
            colliders.CollectionChanged += OnCollectionChanged;
        }
        
        ~ColliderGroup()
        {
            // Un-ignore and remove self from all ignored groups
            foreach (var ignoredGroup in ignoredGroups)
            {
                foreach (var ignoredCollider in ignoredGroup.colliders)
                    foreach (var collider in colliders)
                        Physics.IgnoreCollision(collider, ignoredCollider, false);
                ignoredGroup.ignoredGroups.Remove(this);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Ignore all new collider with all ignored groups
            foreach (Collider newCollider in e.NewItems)
                foreach (var ignoredGroup in ignoredGroups)
                    foreach (var ignoredCollider in ignoredGroup.colliders)
                        Physics.IgnoreCollision(newCollider, ignoredCollider);

            // Un-ignore all removed colliders with all ignored groups
            foreach (Collider oldCollider in e.OldItems)
                foreach (var ignoredGroup in ignoredGroups)
                    foreach (var ignoredCollider in ignoredGroup.colliders)
                        Physics.IgnoreCollision(oldCollider, ignoredCollider, ignore: false);
        }

        // Ignore all colliders in this group with all colliders in the other group
        public void IgnoreCollisionsWith(ColliderGroup otherGroup, bool ignore = true)
        {
            if(ignore)
            {
                ignoredGroups.Add(otherGroup);
                otherGroup.ignoredGroups.Add(this);
            }
            else
            {
                ignoredGroups.Remove(otherGroup);
                otherGroup.ignoredGroups.Remove(this);
            }
            
            foreach (var collider in colliders)
                foreach (var otherCollider in otherGroup.colliders)
                    Physics.IgnoreCollision(collider, otherCollider, ignore);
        }
    }
}