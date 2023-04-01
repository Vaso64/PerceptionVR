using UnityEngine;
using PerceptionVR.Debug;
using UnityEngine.Serialization;


namespace PerceptionVR.Portals
{
    public abstract class CloneBase<T> : MonoBehaviour where T : Component
    {
        [FormerlySerializedAs("target")] [SerializeField] protected T currentTarget;
        protected Portal currentThroughPortal;

        protected bool targetSet;
        
        public virtual void Track(T target, Portal throughPortal)
        {
            this.currentTarget = target;
            this.currentThroughPortal = throughPortal;
            this.targetSet = true;
            
            Debugger.LogInfo($"{this} is tracking {target} through {throughPortal}");
        }
    }
}