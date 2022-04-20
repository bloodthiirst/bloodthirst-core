using Bloodthirst.BType;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class RectPropertyFilter : IBJsonPropertyFilter<Rect>
    {
        internal static readonly List<string> allowedMembers = new List<string>()
        {
            nameof(Rect.x),
            nameof(Rect.y),
            nameof(Rect.width),
            nameof(Rect.height)
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return t == typeof(Rect);
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
