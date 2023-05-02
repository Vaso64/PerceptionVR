using UnityEngine;


namespace PerceptionVR.Portals
{
    public abstract class CloneBase<T>: MonoBehaviourBase where T : Component
    {
        public T currentTarget { get; protected set; }
        protected Portal currentThroughPortal;

        protected bool targetSet;
        
        public virtual void Track(T targetTeleportable, Portal throughPortal)
        {
            this.currentTarget = targetTeleportable;
            this.currentThroughPortal = throughPortal;
            this.targetSet = true;
            
            //Debugger.LogInfo($"{this} is tracking {targetTeleportable} through {throughPortal}");
        }
    }
}