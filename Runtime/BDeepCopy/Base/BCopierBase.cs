using System;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BCopierBase : IBCopierInternal
    {
        Type IBCopier.Type => CopierType();

        internal IBCopier Copier { get; }

        object IBCopier.Copy(object t)
        {
            if (t == null)
                return null;

            return Copy(t, new BCopierContext());
        }

        object IBCopierInternal.Copy(object t, BCopierContext copierContext)
        {
            return Copy(t, copierContext);
        }

        private object Copy(object t, BCopierContext copierContext)
        {
            if (t == null)
                return null;

            if (copierContext.TryGetCached(t, out object cached))
                return cached;

            object emptyCopy = CreateEmptyCopy(t);
            copierContext.Register(t, emptyCopy);

            return Copy(t, emptyCopy, copierContext);
        }

        internal abstract object CreateEmptyCopy(object original);

        internal abstract object Copy(object t, object emptyCopy, BCopierContext copierContext);

        internal abstract Type CopierType();
    }
}
