using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    public class BJsonProvider
    {
        private Dictionary<Type, IBJsonConverterInternal> Converters { get; set; }
        private BJsonFactory Factory { get; set; }
        public BJsonProvider()
        {
            Converters = new Dictionary<Type, IBJsonConverterInternal>();
            Factory = new BJsonFactory();
        }

        public IBJsonConverterInternal GetConverter(Type t)
        {
            if (Converters.TryGetValue(t, out IBJsonConverterInternal cnv))
            {
                return cnv;
            }


            cnv = Factory.CreateConverter(t);
            cnv.Provider = this;
            cnv.Initialize();


            Converters.Add(t, cnv);

            return cnv;
        }
    }
}
