using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BCopierPureStructBase<T> : IBCopierInternal where T : struct
    {
        private static readonly Type type = typeof(T);

        Type IBCopier.Type => type;

        public abstract IReadOnlyList<MemberInfo> CopiableMembers();

        public object Copy(object t, BCopierContext copierContext, BCopierSettings bCopierSettings)
        {
            return t;
        }

        public object Copy(object t, BCopierSettings bCopierSettings = null)
        {
            return t;
        }

    }
}
