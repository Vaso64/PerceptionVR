using UnityEngine;

namespace PerceptionVR.Extensions
{
    public static class RectExtensions
    {
        public static Rect Clamp(this Rect rect, Rect clampRect)
        {
            rect.xMin = rect.min.x < clampRect.min.x ? clampRect.min.x : rect.min.x;
            rect.yMin = rect.min.y < clampRect.min.y ? clampRect.min.y : rect.min.y;
            rect.xMax = rect.max.x > clampRect.max.x ? clampRect.max.x : rect.max.x;
            rect.yMax = rect.max.y > clampRect.max.y ? clampRect.max.y : rect.max.y;
            return rect;
        }

        public static Rect Scale(this Rect rect, Rect scale)
        {
            rect.width *= scale.width;
            rect.height *= scale.height;
            rect.x = scale.x + rect.x * scale.width;
            rect.y = scale.y + rect.y * scale.height;
            return rect;
        }
    }
}
