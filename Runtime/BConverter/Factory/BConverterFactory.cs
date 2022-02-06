using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BConverterFactory<TConverter> : IBConverterFactory where TConverter : IBConverterInternal
    {
        private static Type baseConverterType = typeof(TConverter);
        private List<IBConverterFilter<TConverter>> ConverterFilters { get; set; }
        internal BConverterFactory()
        {
            CreateFilters();
        }

        public void CreateFilters()
        {
            ((IBConverterFactory)this).Initialize();
        }

        void IBConverterFactory.Initialize()
        {
            Type genericType = typeof(IBConverterFilter<>).MakeGenericType(baseConverterType);

            IEnumerable<Type> factories = Assembly.GetAssembly(baseConverterType)
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => TypeUtils.IsSubTypeOf(t, genericType));

            ConverterFilters = new List<IBConverterFilter<TConverter>>();
            foreach (Type t in factories)
            {
                IBConverterFilter<TConverter> filterInstance = (IBConverterFilter<TConverter>)Activator.CreateInstance(t);
                ConverterFilters.Add(filterInstance);
            }
        }

        IBConverterInternal IBConverterFactory.CreateConverter(Type t)
        {
            for (int i = 0; i < ConverterFilters.Count; i++)
            {
                IBConverterFilter<TConverter> curr = ConverterFilters[i];

                if (curr.CanConvert(t))
                {
                    return curr.GetConverter(t);
                }
            }

            return CreateConverterInternal(t);
        }

        protected abstract TConverter CreateConverterInternal(Type t);




    }
}
