using UnityEngine;
using PerceptionVR.Debug;


namespace PerceptionVR.Portals
{
    public abstract class CloneBase<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected T target;
        protected Portal throughPortal;

        protected bool targetSet;
        
        public virtual void Track(T target, Portal throughPortal)
        {
            this.target = target;
            this.throughPortal = throughPortal;
            this.targetSet = true;
            
            Debugger.LogInfo($"{this} is tracking {target} through {throughPortal}");
        }
    }
}