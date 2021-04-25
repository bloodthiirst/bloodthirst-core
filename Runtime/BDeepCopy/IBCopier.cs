using System;

namespace Bloodthirst.BDeepCopy
{
    public interface IBCopier
    {
        Type Type { get; }

        object Copy(object t);
    }

    internal interface IBCopierInternal : IBCopier
    {
        object Copy(object t, BCopierContext copierContext);
    }

    public interface IBCopier<T>
    {
        T Copy(T t);
    }

}
