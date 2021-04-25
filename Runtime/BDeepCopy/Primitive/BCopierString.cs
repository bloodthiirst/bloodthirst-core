using System;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierString : IBCopierInternal
    {
        private static readonly Type type = typeof(string);

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
