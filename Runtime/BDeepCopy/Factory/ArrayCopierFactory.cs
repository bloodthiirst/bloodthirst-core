using System;

namespace Bloodthirst.BDeepCopy
{
    internal class ArrayCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return t.IsArray;
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierArrayType(t);
        }
    }
}
