using System;
using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Puzzle;
using UnityEngine;

namespace PerceptionVR.Levels
{
    public class ThreeButtons : LevelBase
    {
        [Serializable]
        private class ControlGroup
        {
            public bool state;
            public Button[] buttons;
            public Indicator[] indicators;
        } 
        
        [SerializeField] private List<ControlGroup> controlGroups = new ();
        [SerializeField] private List<Door> doors = new ();
        
        private void Awake()
        {
            foreach (var controlGroup in controlGroups)
            foreach (var pressedButton in controlGroup.buttons)
            {
                pressedButton.OnPressed.AddListener(() =>
                {
                    controlGroup.state = !controlGroup.state;
                    foreach (var indicator in controlGroup.indicators)
                        indicator.SetColor(controlGroup.state ? Color.green : Color.red);
                    foreach (var button in controlGroup.buttons) button.SetColor(controlGroup.state ? Color.green : Color.red);
                    foreach (var door in doors) door.SetOpen(controlGroups.All(group => group.state));
                });
            }
        }
    }
}