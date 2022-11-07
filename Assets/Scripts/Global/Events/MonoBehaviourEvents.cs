using System;
using UnityEngine;

namespace PerceptionVR.Global
{
    public class MonoBehaviourEvents : MonoBehaviour
    {
        public static event Action OnAwake;
        public static event Action OnStart;
        public static event Action OnUpdate;
        public static event Action OnFixedUpdate;
        public static event Action OnLateUpdate;

        private void Awake() => OnAwake?.Invoke();
        
        private void Start() => OnStart?.Invoke();
        
        private void Update() => OnUpdate?.Invoke();
        
        private void FixedUpdate() => OnFixedUpdate?.Invoke();
        
        private void LateUpdate() => OnLateUpdate?.Invoke();
    }
}