using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierString : IBCopierInternal
    {
        private static readonly Type type = typeof(string);

        Type IBCopier.Type => type;

        object IBCopierInternal.Copy(object t, BCopierContext copierContext , BCopierSettings copierSettings)
        {
            return t;
        }

        object IBCopier.Copy(object t , BCopierSettings copierSettings)
        {
            return t;
        }
    }
}
