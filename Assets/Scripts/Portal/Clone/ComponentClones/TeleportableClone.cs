using UnityEngine;
using PerceptionVR.Extensions;
using PerceptionVR.Global;
using System.Collections.Generic;
using System.Linq;
using System;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public class TeleportableClone : TrackedCloneBase<Transform>
    {
        public event Action OnTeleport;

        private ITeleportable targetTeleportable;

        private void Awake()
        {
            base.OnEnterPortal += () => OnTeleport?.Invoke();
            OnTeleport += OnEnterPortalCallback;
        }
        
        protected override void Update() => base.Update();
        
        public void Track(ITeleportable target, IPortal throughPortal)
        {
            // Position clone
            transform.SetPose(throughPortal.PairPose(target.transform.GetPose()));
            
            // Track rigidbody
            foreach (var movePair in CreatePairSet<Transform, MovementClone>(target.transform, transform))
                movePair.clone.Track(movePair.original, throughPortal);

            // Track teleportable behaviour
            foreach (var tbPair in CreatePairSet<ITeleportableBehaviour, TeleportableBehaviourClone>(target.transform, transform))
                tbPair.clone.Track(tbPair.original, throughPortal);

            this.targetTeleportable = target;
            base.Track(target.transform, throughPortal);
        }
        
        private void OnEnterPortalCallback()
        {
            Debugger.LogInfo($"{this} ENTERED PORTAL!");
            throughPortal.Teleport(targetTeleportable);
            Track(targetTeleportable, throughPortal.portalPair);
        }
        
        
        private static IEnumerable<(TOriginal original, TClone clone)> CreatePairSet<TOriginal, TClone>(Component originalRoot, Component cloneRoot)
        {
            var originalComponents = originalRoot.GetComponentsInChildren<TOriginal>();
            var cloneComponents = cloneRoot.GetComponentsInChildren<TClone>();
            if(originalComponents.Length != cloneComponents.Length)
                Debugger.LogError($"{originalRoot} and {cloneRoot} have different numbers of {typeof(TOriginal)} / {typeof(TClone)} components");
            return originalComponents.Zip(cloneComponents, (original, clone) => (original, clone));
        }


        public static TeleportableClone CreateClone(ITeleportable original)
        {
            // Clone original
            var clone = GameObjectUtility.InstantiateNotify(original.gameObject);

            IEnumerable<Type> defaultPreservedComponents = new [] { typeof(Rigidbody), typeof(Collider), typeof(MeshRenderer), 
                                                                    typeof(MeshFilter), typeof(Transform) };

            // Iterate through all children
            foreach (var child in clone.GetComponentsInChildren<Transform>())
            {
                var childComponents = child.GetComponents<Component>().ToList();
                var preservedComponents = defaultPreservedComponents;
                
                // Try get TeleportableBehaviour
                var tb = childComponents.OfType<ITeleportableBehaviour>().FirstOrDefault();
                if (tb != null)
                {
                    // Notify teleportableBehaviour
                    tb.OnCreateClone(child.gameObject, out var extraPreservedComponents);
                    preservedComponents = preservedComponents.Concat(extraPreservedComponents);
                }
                
                // Strip unneeded components
                foreach (var component in childComponents.ToArray())
                {
                    var componentType = component.GetType();
                    if (!preservedComponents.Any(preserveComp => preserveComp == componentType || componentType.IsSubclassOf(preserveComp)))
                    {
                        GameObjectUtility.DestroyComponentNotify(component);
                        childComponents.Remove(component);
                    }
                }
                
                // Set-up Movement clone
                child.gameObject.AddComponentNotify<MovementClone>();
                
                // Set-up TeleportableBehaviour clone
                if(tb != null)
                    child.gameObject.AddComponentNotify<TeleportableBehaviourClone>();
                
                child.name += "(Clone)";
            }
            
            Debugger.LogInfo($"{original.transform} was cloned.");
            
            return clone.AddComponentNotify<TeleportableClone>();
        }
    }
}