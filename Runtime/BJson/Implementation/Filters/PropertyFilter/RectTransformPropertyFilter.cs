using Bloodthirst.BType;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class RectTransformPropertyFilter : IBJsonPropertyFilter<RectTransform>
    {
        internal static readonly List<string> allowedMembers = new List<string>()
        {
            nameof(RectTransform.anchorMin),
            nameof(RectTransform.anchorMax),
            nameof(RectTransform.offsetMin),
            nameof(RectTransform.offsetMax),

            nameof(RectTransform.anchoredPosition),
            nameof(RectTransform.sizeDelta),
            nameof(RectTransform.pivot),

            nameof(RectTransform.localPosition),
            nameof(RectTransform.localRotation),
            nameof(RectTransform.localScale),
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return t == typeof(RectTransform);
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
