using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.BDeepCopy
{
    internal class TypeCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return t == typeof(Type);
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierPureValueType(t);
        }
    }
}
