using System;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BCopierPureStructBase<T> : IBCopierInternal where T : struct
    {
        private static readonly Type type = typeof(T);

        Type IBCopier.Type => type;

        object IBCopierInternal.Copy(object t, BCopierContext copierContext)
        {
            return t;
        }

        object IBCopier.Copy(object t)
        {
            return t;
        }
    }
}
