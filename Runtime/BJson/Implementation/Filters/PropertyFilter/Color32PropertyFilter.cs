using Bloodthirst.BType;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class Color32PropertyFilter : IBJsonPropertyFilter<Color32>
    {
        internal static readonly List<string> allowedMembers = new List<string>()
        {
            nameof(Color.r),
            nameof(Color.g),
            nameof(Color.b),
            nameof(Color.a)
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return t == typeof(Color32);
        }

        BTypeData IBJsonPropertyFilter.FilteredProperties(BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for (int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                if (!allowedMembers.Contains(curr.MemberInfo.Name))
                    cpy.MemberDatas.RemoveAt(i);
            }

            return cpy;
        }
    }
}
