using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierPureValueType : IBCopierInternal
    {
        private readonly Type t;

        private object DefaultValue { get; set; }

        internal BCopierPureValueType(Type t)
        {
            this.t = t;

            Func<object> defaultValueFunc = ReflectionUtils.GetDefaultValue(t);

            DefaultValue = defaultValueFunc();
        }

        Type IBCopier.Type => t;

        public object Copy(object t, BCopierContext copierContext, BCopierSettings bCopierSettings)
        {
            return t;
        }

        public object Copy(object t, BCopierSettings bCopierSettings = null)
        {
            return t;
        }

        object IBCopierInternal.GetDefaultValue()
        {
            return DefaultValue;
        }
    }
}
