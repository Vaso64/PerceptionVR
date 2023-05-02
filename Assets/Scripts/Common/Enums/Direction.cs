using System;
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

        public static Direction ToDirection(this Vector3 vector3)
        {
            var normalized = vector3.normalized;

            if(normalized == Vector3.up)       return Direction.Up;
            if(normalized == Vector3.down)     return Direction.Down;
            if(normalized == Vector3.left)     return Direction.Left;
            if(normalized == Vector3.right)    return Direction.Right;
            if(normalized == Vector3.forward)  return Direction.Forward;
            if(normalized == Vector3.back)     return Direction.Backward;

            throw new ArgumentException("Vector3 is not a direction");
        }
    }
}