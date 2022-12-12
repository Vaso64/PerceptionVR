using System;
using PerceptionVR.Debug;
using UnityEngine;

namespace PerceptionVR.Puzzle
{
    // TODO Gravity independent
    [RequireComponent(typeof(ConfigurableJoint))]
    public class PressurePlate : BoolControlBase
    {
        [SerializeField] private Transform plate;
        
        public float plateHeight = 0.3f;
        public float plateOffset = 0.15f;
        public float pressure = 10f;
        
        private bool _wasPressedLastFrame;

        public void Awake()
        {
            var joint = GetComponent<ConfigurableJoint>();
            joint.anchor = new Vector3(0, plateHeight, 0);
            joint.linearLimit = new SoftJointLimit { limit = plateOffset };
            joint.yDrive = new JointDrive { positionSpring = pressure, positionDamper = 1f, maximumForce = Mathf.Infinity };
            
            OnActivated.AddListener(() => Debugger.LogInfo("activated"));
            OnDeactivated.AddListener(() => Debugger.LogInfo("deactivated"));
        }
        
        public void Update()
        {
            var isPressed = plate.localPosition.y < plateHeight;
            if (!_wasPressedLastFrame && isPressed)
            {
                isActivated = true;
                OnActivated.Invoke();
                OnChanged.Invoke(true);
            }
            else if (_wasPressedLastFrame && !isPressed)
            {
                isActivated = false;
                OnDeactivated.Invoke();
                OnChanged.Invoke(false);
            }
            _wasPressedLastFrame = isPressed;
        }
    }
}