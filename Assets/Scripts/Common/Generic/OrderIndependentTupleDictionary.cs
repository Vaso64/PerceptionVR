using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PerceptionVR.Common.Generic
{
    public class OrderIndependentTupleDictionary<TKey, TValue> : Dictionary<(TKey, TKey), TValue> where TKey : Object
    {
        public OrderIndependentTupleDictionary() : base(new OrderIndependentTupleEqualityComparer<TKey>()){ }
    }
    
    public class OrderIndependentTupleEqualityComparer<TKey> : IEqualityComparer<(TKey, TKey)> where TKey : Object
    {
        public bool Equals((TKey, TKey) x, (TKey, TKey) y) => Order(x) == Order(y);

        public int GetHashCode((TKey, TKey) x)
        {
            x = Order(x);
            return HashCode.Combine(x.Item1.GetHashCode(), x.Item2.GetHashCode());
        }
        
        private static (TKey, TKey) Order((TKey, TKey) pair) => pair.Item1.GetInstanceID() < pair.Item2.GetInstanceID() ? pair : (pair.Item2, pair.Item1);
    }
}