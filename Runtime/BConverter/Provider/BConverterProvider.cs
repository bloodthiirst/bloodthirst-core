using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BConverterProvider<TConverter> : IBConverterProvider where TConverter : IBConverterInternal
    {
        protected Dictionary<Type, IBConverterInternal> Converters { get; } = new Dictionary<Type, IBConverterInternal>();

        public IBConverterFactory Factory { get; private set; }

        internal BConverterProvider(IBConverterFactory factory)
        {
            Factory = factory;
        }

        protected void Add(TConverter converter)
        {
            converter.Provider = this;
            converter.Initialize();
            Converters.Add(converter.FromType , converter);
        }

        internal TConverter GetOrCreate(Type t)
        {
            if (!Converters.TryGetValue(t, out IBConverterInternal c))
            {
                c = Factory.CreateConverter(t);

                Converters.Add(t, c);
                
                c.Provider = this;
                c.Initialize();
                
                return (TConverter)c;
            }


            return (TConverter)c;
        }

        protected TConverter Get(Type t)
        {
            return GetOrCreate(t);
        }

        IBConverter IBConverterProvider.Get(Type t)
        {
            return Get(t);
        }
    }
}
