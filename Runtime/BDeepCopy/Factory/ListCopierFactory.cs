using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class ListCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(IList));
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierIListType(t);
        }
    }
}
