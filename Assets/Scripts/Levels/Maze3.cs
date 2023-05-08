using System.Linq;
using PerceptionVR.Puzzle;
using UnityEngine;

namespace PerceptionVR.Levels
{
    public class Maze3 : LevelBase
    {
        [SerializeField] private Button[] maze4pressurePlates;
        [SerializeField] private Door maze4door;
        
        [SerializeField] private Button maze1pressurePlate;
        [SerializeField] private Door maze1door;
        

        private void Start()
        {
            foreach (var pressurePlate in maze4pressurePlates)
                pressurePlate.OnChanged.AddListener(_ =>
                {
                    pressurePlate.SetColor(pressurePlate.isPressed ? Color.green : Color.red);
                    maze4door.SetOpen(maze4pressurePlates.All(p => p.isPressed));
                });

            maze1pressurePlate.OnChanged.AddListener(_ =>
            {
                maze1pressurePlate.SetColor(maze1pressurePlate.isPressed ? Color.green : Color.red);
                maze1door.SetOpen(maze1pressurePlate.isPressed);
            });
        }
    }
}