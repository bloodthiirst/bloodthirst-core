using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    public class BJsonSettings
    {
        public object CustomContext { get; set; }
        public bool Formated { get; set; }
        public BJsonProvider Provider { get; set; }
        public List<IBJsonFilterInternal> CustomConverters { get; set; } = new List<IBJsonFilterInternal>();

        private Dictionary<Type, IBJsonConverterInternal> Converters { get; set; } = new Dictionary<Type, IBJsonConverterInternal>();

        public BJsonSettings()
        {

        }

        public BJsonSettings(List<IBJsonFilterInternal> customConverters)
        {
            CustomConverters = customConverters;
        }

        public bool HasCustomConverter(Type t, out IBJsonConverterInternal converter)
        {
            if (Converters.TryGetValue(t, out converter))
            {
                return true;
            }

            for (int i = 0; i < CustomConverters.Count; i++)
            {
                IBJsonFilterInternal curr = CustomConverters[i];

                if (curr.CanConvert(t))
                {
                    converter = curr.GetConverter_Internal(t);
                    converter.Provider = Provider;
                    converter.Initialize();

                    Converters.Add(t , converter);
                    
                    return true;
                }
            }

            converter = null;
            return false;
        }
    }
}
