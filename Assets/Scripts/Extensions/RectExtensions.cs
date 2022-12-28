using UnityEngine;

namespace PerceptionVR.Extensions
{ 
    public static class RectExtensions
    {
        public static Rect IntersectionWith(this Rect thisRect, Rect otherRect)
        {
            thisRect.xMin = thisRect.min.x > otherRect.min.x ? thisRect.min.x : otherRect.min.x;
            thisRect.yMin = thisRect.min.y > otherRect.min.y ? thisRect.min.y : otherRect.min.y;
            thisRect.xMax = thisRect.max.x < otherRect.max.x ? thisRect.max.x : otherRect.max.x;
            thisRect.yMax = thisRect.max.y < otherRect.max.y ? thisRect.max.y : otherRect.max.y;       
            return thisRect;
        }
    }
}
