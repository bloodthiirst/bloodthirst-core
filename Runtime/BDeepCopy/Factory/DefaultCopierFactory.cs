using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class DefaultCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return true;
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierConcreteTypeComplex(t);
        }
    }
}
