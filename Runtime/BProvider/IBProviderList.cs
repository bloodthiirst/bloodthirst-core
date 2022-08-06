using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    public interface IBProviderList
    {
        ICollection Elements { get; }
        void Add(object element);
        bool Remove(object element);
    }
}