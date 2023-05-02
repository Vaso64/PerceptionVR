using System.Collections.Generic;
using System.Linq;
using PerceptionVR.Debug;
using UnityEngine;

namespace PerceptionVR.Portals
{
    public static class SwapUtility
    {
        // Swap every item in collection with tuple (Item1 -> Item2, Item2 -> Item1)
        public static void PerformSwap<T>(ICollection<T> collection, IEnumerable<(T, T)> swaps)
        {
            var set = new HashSet<T>(collection);
            foreach (var (a, b) in swaps)
            {
                if (set.Contains(a))
                {
                    collection.Remove(a);
                    collection.Add(b);
                }
                else if (set.Contains(b))
                {
                    collection.Remove(b);
                    collection.Add(a);
                }
            }
        }
        
        public static void PerformSwap<T>(ICollection<T> collection, IEnumerable<(T, T)> swaps, out IEnumerable<(T removed, T added)> appliedSwaps)
        {
            appliedSwaps = GetApplicableSwaps(collection, swaps).ToList();

            foreach (var (toRemove, toAdd) in appliedSwaps)
            {
                collection.Remove(toRemove);
                collection.Add(toAdd);
            }
        }
        
        public static IEnumerable<(T toRemove, T toAdd)> GetApplicableSwaps<T>(ICollection<T> collection, IEnumerable<(T, T)> swaps)
        {
            var set = new HashSet<T>(collection);
            foreach (var (a, b) in swaps)
            {
                var aInSet = set.Contains(a);
                var bInSet = set.Contains(b);
                if (aInSet && !bInSet)
                    yield return (a, b);
                else if (bInSet && !aInSet)
                    yield return (b, a);
            }
        }
        
        // Create list of associated component tuples
        // Clone object must be cloned from original object
        public static IEnumerable<(TOriginal original, TClone clone)> CreateSwaps<TOriginal, TClone>(Component originalRoot, Component cloneRoot) 
        {
            var originalComponents = originalRoot.GetComponentsInChildren<TOriginal>();
            var cloneComponents = cloneRoot.GetComponentsInChildren<TClone>();
            if(originalComponents.Length != cloneComponents.Length)
                Debugger.LogError($"{originalRoot} and {cloneRoot} have different numbers of {typeof(TOriginal)} and {typeof(TClone)} components");
            return originalComponents.Zip(cloneComponents, (original, clone) => (original, clone));
        }
    }
}