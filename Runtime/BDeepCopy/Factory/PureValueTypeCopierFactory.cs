using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.BDeepCopy
{
    internal class PureValueTypeCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return TypeUtils.IsPureValueType(t);
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierPureValueType(t);
        }
    }
}
