using Bloodthirst.BType;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class TransformPropertyFilter : IBJsonPropertyFilter<Transform>
    {
        internal static readonly List<string> allowedMembers = new List<string>()
        {
            nameof(Transform.localPosition),
            nameof(Transform.localRotation),
            nameof(Transform.localScale),
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return t == typeof(Transform);
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
