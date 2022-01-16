using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal static class BTypeProvider
    {
        private static Dictionary<Type, BTypeData> TypeDatas { get; set; } = new Dictionary<Type, BTypeData>();

        internal static BTypeData GetOrCreate(Type t)
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
