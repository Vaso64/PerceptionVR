using System;
using System.Collections.Generic;

namespace PerceptionVR.Common.Generic
{
    public interface INotifyCollectionChanged<out T>
    {
        event Action<IEnumerable<T>> OnAdded;
        event Action<IEnumerable<T>> OnRemoved;
    }
}