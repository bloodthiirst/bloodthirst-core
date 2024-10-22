﻿using Bloodthirst.BType;
using System;

namespace Bloodthirst.BJson
{
    internal class JsonIgnorePropertyFilter : IBJsonPropertyFilter
    {
        private static Type ignoreType = typeof(BJsonIgnoreAttribute);
        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return true;
        }

        BTypeData IBJsonPropertyFilter.FilteredProperties(BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for (int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                if (curr.InheritedAttributes.ContainsKey(ignoreType))
                {
                    cpy.MemberDatas.RemoveAt(i);
                }
            }

            return cpy;
        }
    }
}
