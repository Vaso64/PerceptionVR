using UnityEngine;

namespace PerceptionVR.Common
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }
    
    public static class DirectionExtensions
    {
        public static Vector3 ToVector3(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:       return Vector3.up;
                case Direction.Down:     return Vector3.down;
                case Direction.Left:     return Vector3.left;
                case Direction.Right:    return Vector3.right;
                case Direction.Forward:  return Vector3.forward;
                case Direction.Backward: return Vector3.back;
                default:                 return Vector3.zero;
            }
        }
    }
}