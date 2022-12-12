using System.Collections.Generic;
using PerceptionVR.Common.Generic;
using PerceptionVR.Global;
using System.Linq;
using UnityEngine;

namespace PerceptionVR.Physics
{
    public static class CollisionMatrix
    {
        private static readonly OrderIndependentTupleDictionary<Collider, bool> Matrix = new();
        
        
        static CollisionMatrix()
        {
            // Set default values
            ComponentTracking.allColliders.OnRemoved += (colliders) =>
            {
                foreach (var toDelete in Matrix.Keys.Where(key => colliders.Contains(key.Item1) || colliders.Contains(key.Item2)).ToList())
                    Matrix.Remove(toDelete);
            };
        }

        public static void SetCollision(Collider x, Collider y, bool collide)
        {
            // Don't include triggers and static object into collision matrix (for now)
            if(x.isTrigger || y.isTrigger || x.gameObject.isStatic || y.gameObject.isStatic)
                return;
            
            if (Matrix.ContainsKey((x,y)))
                Matrix[(x,y)] = collide;
            else
                Matrix.Add((x,y), collide);
            //Debugger.LogInfo($"{x} - {y} => Collide: {collide}");
            UnityEngine.Physics.IgnoreCollision(x, y, !collide);
        }

        public static void SetCollisions(IEnumerable<Collider> xs, IEnumerable<Collider> ys, bool collide)
        {
            foreach (var x in xs)
            foreach (var y in ys)
                SetCollision(x, y, collide);
        }
    }
}