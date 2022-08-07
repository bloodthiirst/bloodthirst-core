using System;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    public interface IBProviderList
    {
        event Action<object> OnAdded;
        event Action<object> OnRemoved;
        ICollection Elements { get; }
        void Add(object element);
        bool Remove(object element);
    }
}