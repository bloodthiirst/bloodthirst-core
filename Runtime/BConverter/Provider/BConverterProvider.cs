using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.BDeepCopy
{
    public abstract class BConverterProvider<TConverter> where TConverter : IBConverter
    {
        private Dictionary<Type, IBConverterInternal> Converters { get; } = new Dictionary<Type, IBConverterInternal>();

        private IBConverterFactory Factory;

        internal BConverterProvider()
        {
            Factory = new BConverterFactory<TConverter>();
        }

        internal TConverter GetOrCreate(Type t)
        {
            if (!Converters.TryGetValue(t, out IBConverterInternal c))
            {
                c = (IBConverterInternal) Factory.CreateConverter(t);
                return (TConverter) c;
            }

            return (TConverter) c;
        }

        protected TConverter Get(Type t)
        {
            return GetOrCreate(t);
        }

    }
}
