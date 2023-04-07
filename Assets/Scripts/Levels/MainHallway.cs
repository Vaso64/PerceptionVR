using System.Linq;
using PerceptionVR.Levels;
using PerceptionVR.Puzzle;
using UnityEngine;

public class MainHallway : LevelBase
{
    [SerializeField] private Button pressurePlate1;
    [SerializeField] private Button pressurePlate2;
    [SerializeField] private Button pressurePlate3;
    
    [SerializeField] private Indicator indicator1;
    [SerializeField] private Indicator indicator2;
    [SerializeField] private Indicator indicator3;

    [SerializeField] private Door door;

    private void Awake()
    {
        // Light up indicators
        pressurePlate1.OnChanged.AddListener(active => indicator1.SetColor(active ? Color.green : Color.red));
        pressurePlate2.OnChanged.AddListener(active => indicator2.SetColor(active ? Color.green : Color.red));
        pressurePlate3.OnChanged.AddListener(active => indicator3.SetColor(active ? Color.green : Color.red));

        // Open door
        foreach (var pressurePlate in new [] {pressurePlate1, pressurePlate2, pressurePlate3})
            pressurePlate.OnChanged.AddListener(_ => door.SetOpen(new [] {pressurePlate1, pressurePlate2, pressurePlate3}.All(p => p.isPressed)));
    }
}
