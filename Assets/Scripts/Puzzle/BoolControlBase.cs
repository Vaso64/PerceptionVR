using UnityEngine;
using UnityEngine.Events;

namespace PerceptionVR.Puzzle
{
    public class BoolControlBase : MonoBehaviour
    {
        public bool isActivated { get; protected set; }
        public UnityEvent OnActivated;
        public UnityEvent OnDeactivated;
        public UnityEvent<bool> OnChanged;
    }
}