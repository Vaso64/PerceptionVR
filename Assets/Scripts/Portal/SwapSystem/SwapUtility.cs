using System.Collections.Generic;

namespace PerceptionVR.Portal
{
    public static class SwapUtility
    {
        // Swap every item in the tuple which is in the collection with second tuple value
        public static void PerformSwap<T>(ICollection<T> collection, IEnumerable<(T,T)> swaps) => PerformSwap(collection, swaps, out _);
        public static void PerformSwap<T>(ICollection<T> collection, IEnumerable<(T,T)> swaps, out IEnumerable<(T removed, T added)> appliedSwaps)
        {
            appliedSwaps = GetApplicableSwaps(collection, swaps);
            foreach (var (toRemove, toAdd) in appliedSwaps)
            {
                collection.Remove(toRemove);
                collection.Add(toAdd);
            }
        }
        
        public static IEnumerable<(T toRemove, T toAdd)> GetApplicableSwaps<T>(ICollection<T> collection, IEnumerable<(T,T)> swaps)
        {
            var applicable = new List<(T, T)>();
            foreach (var (a, b) in swaps)
            {
                if (collection.Contains(a))
                    applicable.Add((a, b));
                else if (collection.Contains(b))
                    applicable.Add((b, a));
            }
            return applicable;
        }
    }
}