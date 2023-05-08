using System.Linq;
using PerceptionVR.Puzzle;
using UnityEngine;

namespace PerceptionVR.Levels
{
    public class Maze1 : LevelBase
    {
        [SerializeField] private Button[] pressurePlates;
        [SerializeField] private Indicator[] indicators;
        [SerializeField] private Door exitDoor;

        private void Start()
        {
            foreach (var controlGroup in pressurePlates.Zip(indicators, (pressurePlate, indicator) => (pressurePlate, indicator)))
            {
                controlGroup.pressurePlate.OnChanged.AddListener(pressed =>
                {
                    controlGroup.indicator.SetColor(pressed ? Color.green : Color.red);
                    controlGroup.pressurePlate.SetColor(pressed ? Color.green : Color.red);
                    exitDoor.SetOpen(pressurePlates.All(p => p.isPressed));
                });
            }
        }
    }
}