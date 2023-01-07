using System;
using System.Collections;
using System.Collections.Generic;
using PerceptionVR.Levels;
using PerceptionVR.Puzzle;
using UnityEngine;

public class MainHallway : LevelBase
{
    [SerializeField] private HoldPressureButton pressurePlate1;
    [SerializeField] private HoldPressureButton pressurePlate2;
    [SerializeField] private HoldPressureButton pressurePlate3;
    
    [SerializeField] private Indicator indicator1;
    [SerializeField] private Indicator indicator2;
    [SerializeField] private Indicator indicator3;

    [SerializeField] private Door door;

    private void Awake()
    {
        // Light up indicators
        pressurePlate1.OnChanged.AddListener(active => indicator1.SetActive(active));
        pressurePlate2.OnChanged.AddListener(active => indicator2.SetActive(active));
        pressurePlate3.OnChanged.AddListener(active => indicator3.SetActive(active));

        // Open door
        foreach (var indicator in new [] {indicator1, indicator2, indicator3})
            indicator.OnChanged.AddListener(_ => door.SetActive(!(indicator1.isActivated && indicator2.isActivated && indicator3.isActivated)));
    }
}
