using System;
using System.Collections.Generic;

namespace Bloodthirst.BType
{
    public static class BTypeProvider
    {
        private static Dictionary<Type, BTypeData> TypeDatas { get; set; } = new Dictionary<Type, BTypeData>();

        public static BTypeData GetOrCreate(Type t)
        {
            if (!TypeDatas.TryGetValue(t, out BTypeData td))
            {
                td = new BTypeData(t);
                return td;
            }

            return td;
        }

        internal static void Register(BTypeData typeData)
        {
            TypeDatas.Add(typeData.Type, typeData);
        }
    }
}
