using System;
using System.Linq;
using PerceptionVR.Global;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public class PhysicsSettings : MonoBehaviour
    {
        [SerializeField] private float _gravityStrength = 9.81f;
        public static float gravityStrength { get; private set; }

        [SerializeField] private int _nFrames;
        public static int nFrames { get; private set; }
        
        [SerializeField] private float _timeToSleep;
        public static float timeToSleep { get; private set; }
        

        [SerializeField] private float _sleepThreshold;
        public static float sleepThreshold { get; private set; }

        [SerializeField] private bool _debugSleep;
        public static bool debugSleep { get; private set; }

        private void Awake() => SetValues();
        private void Update() => SetValues();

        private void SetValues()
        {
            gravityStrength = _gravityStrength;
            nFrames = _nFrames;
            timeToSleep = _timeToSleep;
            sleepThreshold = _sleepThreshold;
            debugSleep = _debugSleep;
        }
    }
}