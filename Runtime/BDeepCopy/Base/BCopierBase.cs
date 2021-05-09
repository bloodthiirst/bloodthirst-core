using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BCopierBase : IBCopierInternal
    {
        internal static List<MemberInfo> EmptyMembers = new List<MemberInfo>();

        Type IBCopier.Type => CopierType();

        internal IBCopier Copier { get; }

        private Func<object> DefaultValueFunc { get; set; }

        protected virtual void Initialize()
        {
            DefaultValueFunc = ReflectionUtils.GetDefaultValue(CopierType());
        }

        public object GetDefaultValue()
        {
            return DefaultValueFunc();
        }

        object IBCopier.Copy(object t, BCopierSettings copierSettings)
        {
            if (t == null)
                return null;

            return Copy(t, new BCopierContext() , copierSettings);
        }

        object IBCopierInternal.Copy(object t, BCopierContext copierContext, BCopierSettings copierSettings)
        {
            return Copy(t, copierContext , copierSettings);
        }

        private object Copy(object t, BCopierContext copierContext , BCopierSettings copierSettings)
        {
            if (t == null)
                return null;

            if (copierContext.TryGetCached(t, out object cached))
                return cached;

            object emptyCopy = CreateEmptyCopy(t);
            copierContext.Register(t, emptyCopy);

            return Copy(t, emptyCopy, copierContext , copierSettings);
        }

        internal abstract object CreateEmptyCopy(object original);

        internal abstract object Copy(object t, object emptyCopy, BCopierContext copierContext , BCopierSettings copierSettings);

        internal abstract Type CopierType();


    }
}
