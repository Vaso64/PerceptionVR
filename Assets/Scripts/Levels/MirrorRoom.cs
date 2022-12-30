using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Puzzle;
using UnityEngine;

namespace PerceptionVR.Levels
{
    public class MirrorRoom : LevelBase
    {
        [SerializeField] private TogglePressureButton button_1A;
        [SerializeField] private TogglePressureButton button_1B;
        [SerializeField] private TogglePressureButton button_2;
        [SerializeField] private TogglePressureButton button_3A;
        [SerializeField] private TogglePressureButton button_3B;
        
        [SerializeField] private Indicator indicator_1A;
        [SerializeField] private Indicator indicator_1B;
        [SerializeField] private Indicator indicator_2A;
        [SerializeField] private Indicator indicator_2B;
        [SerializeField] private Indicator indicator_3A;
        [SerializeField] private Indicator indicator_3B;

        [SerializeField] private ControlBase door_A;
        [SerializeField] private ControlBase door_B;
        

        private void Awake()
        {
            // Set indicators
            button_1A.OnChanged.AddListener(indicator_1A.SetActive);
            button_1B.OnChanged.AddListener(indicator_1B.SetActive);
            button_2.OnChanged.AddListener(indicator_2A.SetActive);
            button_2.OnChanged.AddListener(indicator_2B.SetActive);
            button_3A.OnChanged.AddListener(indicator_3A.SetActive);
            button_3B.OnChanged.AddListener(indicator_3B.SetActive);
            
            
            // Mirror button
            button_1A.OnChanged.AddListener(active => MirrorButton(button_1B, active));
            button_1B.OnChanged.AddListener(active => MirrorButton(button_1A, active));
            button_3A.OnChanged.AddListener(active => MirrorButton(button_3B, active));
            button_3B.OnChanged.AddListener(active => MirrorButton(button_3A, active));
            
            
            // Open doors//
           //indicator_1A.OnChanged.AddListener(_ => RefreshDoors(new []{indicator_1A, indicator_2A, indicator_3A}, door_A));
           //indicator_2A.OnChanged.AddListener(_ => RefreshDoors(new []{indicator_1A, indicator_2A, indicator_3A}, door_A));
           //indicator_3A.OnChanged.AddListener(_ => RefreshDoors(new []{indicator_1A, indicator_2A, indicator_3A}, door_A));
           //
           //indicator_1B.OnChanged.AddListener(_ => RefreshDoors(new []{indicator_1B, indicator_2B, indicator_3B}, door_B));
           //indicator_2B.OnChanged.AddListener(_ => RefreshDoors(new []{indicator_1B, indicator_2B, indicator_3B}, door_B));
           //indicator_3B.OnChanged.AddListener(_ => RefreshDoors(new []{indicator_1B, indicator_2B, indicator_3B}, door_B));
        }
        
        private static void MirrorButton(TogglePressureButton button, bool active)
        {
            if(button.isActivated != active)
                button.SetActive(active);
        }

        private static void RefreshDoors(IEnumerable<Indicator> indicators, ControlBase door) => door.SetActive(indicators.All(x => x.isActivated));
    }
}