using System;

namespace Bloodthirst.BDeepCopy
{
    internal class InterfaceCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return t.IsInterface;
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierInterfaceType(t);
        }
    }
}
