using System;
using System.Collections;
using UnityEngine;

namespace PerceptionVR.Global
{
    public class MonoBehaviourEvents: MonoBehaviourBase
    {
        public static event Action OnAwake;
        public static event Action OnStart;
        public static event Action OnUpdate;
        public static event Action OnFixedUpdate;
        public static event Action OnLateUpdate;
        public static event Action LateFixedUpdate;

        private void Awake()
        { 
            OnAwake?.Invoke();
            StartCoroutine(LateFixedUpdateCoroutine());
        }
        
        private void Start() => OnStart?.Invoke();
        
        private void Update() => OnUpdate?.Invoke();
        
        private void FixedUpdate() => OnFixedUpdate?.Invoke();
        
        private void LateUpdate() => OnLateUpdate?.Invoke();
        
        private static IEnumerator LateFixedUpdateCoroutine()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                LateFixedUpdate?.Invoke();
            }
        }
    }
}