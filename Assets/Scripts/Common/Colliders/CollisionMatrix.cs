using System.Collections.Generic;
using PerceptionVR.Common.Generic;
using UnityEngine;

namespace PerceptionVR.Common
{
    public static class CollisionMatrix
    {
        private static readonly OrderIndependentTupleDictionary<Collider, bool> _collisionMatrix = new();

        public static void SetCollision(Collider x, Collider y, bool collide)
        {

        }

        public static void SetCollisions(IEnumerable<Collider> xs, IEnumerable<Collider> ys, bool collide)
        {
            foreach (var x in xs)
            foreach (var y in ys)
                SetCollision(x, y, collide);
        }
    }
}