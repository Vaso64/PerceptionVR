using UnityEngine;
using UnityEngine.Events;

namespace PerceptionVR.Puzzle
{
    public class ControlBase: MonoBehaviourBase
    {
        public bool isActivated { get; private set; }
        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;
        public UnityEvent<bool> OnChanged;
        
        public void SetActive(bool active)
        {
            if (active == isActivated)
                return;
            
            isActivated = active;
            OnChanged?.Invoke(active);
            if (active)
                OnActivated?.Invoke();
            else
                OnDeactivated?.Invoke();
        }
    }
}