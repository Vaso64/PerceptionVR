using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using System.Collections.Generic;
using System.Linq;
using MoreLinq.Extensions;
using System;

namespace PerceptionVR.Portal
{
    public class TeleportableClone : CloneBase, ITeleportable
    {
        private ITeleportable targetTeleportable;

        private void Awake()
        {
            base.OnEnterPortal += OnEnterPortalCallback;
        }

        // Start tracking coroutine
        public void Track(ITeleportable teleportable, IPortal portal)
        {
            targetTeleportable = teleportable;
            
            // Position clone
            transform.SetPose(portal.PairPose(teleportable.transform.GetPose()));

            base.Track(teleportable, portal);
            
            // Start tracking coroutine in subTeleportables
            foreach (var subTeleportable in teleportable.transform.GetComponentsInChildren<ISubTeleportable>())
                transform.Find(subTeleportable.transform.GetPath(relativeTo: teleportable.transform)).gameObject
                    .GetComponent<SubTeleportableClone>().Track(subTeleportable, portal);
        }

        private void OnEnterPortalCallback()
        {
            targetPortal.Teleport(targetTeleportable);
            Track(targetTeleportable, targetPortal.portalPair);
            GetComponentsInChildren<SubTeleportableClone>().ForEach(x => x.SwapBehaviour());
        }

        public static TeleportableClone CreateClone(ITeleportable original, IPortal portal)
        {
            // Clone original and create nearby teleportable entity
            var clone = GameObjectUtility.InstantiateNotify(original.transform.gameObject);

            // Strip clone from unnecessary components
            var defaultPreserveComponents = new List<Type> { typeof(Rigidbody), typeof(Collider), typeof(MeshRenderer), typeof(MeshFilter), 
                                                             typeof(Transform), typeof(TeleportableClone), typeof(SubTeleportableClone) };
            foreach (var child in clone.GetComponentsInChildren<Transform>())
            {
                var preserveComponents = child.GetComponent<ITeleportableBase>()?.GetPreservedComponents().Concat(defaultPreserveComponents) ?? defaultPreserveComponents;
                child.GetComponents(typeof(Component))
                    .Where(comp => !preserveComponents
                        .Any(preserveComp => comp.GetType() == preserveComp || comp.GetType().IsSubclassOf(preserveComp)))
                    .ForEach(x => child.gameObject.RemoveComponentNotify(x));
            }

            // Setup tracking
            var teleportableClone = clone.AddComponent<TeleportableClone>();
            foreach (var subOriginal in original.transform.GetComponentsInChildren<ISubTeleportable>())
                clone.transform.Find(subOriginal.transform.GetPath(relativeTo: original.transform)).gameObject
                    .AddComponentNotify<SubTeleportableClone>();
            
            // Start tracking
            teleportableClone.Track(original, portal);  

            return teleportableClone;
        }
        
    }
}