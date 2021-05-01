using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class ListCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return typeof(IList).IsAssignableFrom(t);
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierIListType(t);
        }
    }
}
