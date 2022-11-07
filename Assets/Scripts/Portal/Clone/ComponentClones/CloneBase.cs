using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portal
{
    public abstract class CloneBase<T> : MonoBehaviour where T : Component
    {
        protected T target;
        protected IPortal throughPortal;

        protected bool targetSet;
        
        public virtual void Track(T target, IPortal throughPortal)
        {
            this.target = target;
            this.throughPortal = throughPortal;
            this.targetSet = true;
            
            Debugger.LogInfo($"{this} is tracking {target} through {throughPortal}");
        }
    }
}