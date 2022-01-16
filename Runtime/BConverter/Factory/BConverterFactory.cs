using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class BConverterFactory<TConverter> : IBConverterFactory where TConverter : IBConverter
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

        IBConverter IBConverterFactory.CreateConverter(Type t)
        {
            for (int i = 0; i < ConverterFilters.Count; i++)
            {
                IBConverterFilter curr = ConverterFilters[i];

                if (curr.CanConvert(t))
                    return curr.GetConverter(t);
            }

            return null;
        }



    }
}
