using System;

namespace Bloodthirst.BDeepCopy
{
    internal interface IBCopyFactory
    {
        bool CanCopy(Type t);

        IBCopierInternal GetCopier(Type t);
    }
}
