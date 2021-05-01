using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public interface IBCopier
    {
        Type Type { get; }

        object Copy(object t , BCopierSettings bCopierSettings = null);

        IReadOnlyList<MemberInfo> CopiableMembers();
    }

    internal interface IBCopierInternal : IBCopier
    {
        object Copy(object t, BCopierContext copierContext , BCopierSettings bCopierSettings);
    }

    public interface IBCopier<T>
    {
        T Copy(T t, BCopierSettings bCopierSettings = null);
    }

}
