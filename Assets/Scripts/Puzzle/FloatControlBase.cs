using UnityEngine;
using UnityEngine.Events;

namespace PerceptionVR.Puzzle
{
    public class FloatControlBase : MonoBehaviour
    {
        public float currentValue { get; protected set; }
        public UnityAction<float> onValueChanged;
    }
}