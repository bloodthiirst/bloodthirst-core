using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Bloodthirst.BJson
{
    internal class LayoutGroupPropertyFilter : IBJsonPropertyFilter<LayoutGroup>
    {
        internal static readonly List<string> ignoredMembers = new List<string>()
        {
            "m_Tracker"
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(LayoutGroup));
        }

        BTypeData IBJsonPropertyFilter.FilteredProperties(BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for (int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                if (ignoredMembers.Contains(curr.MemberInfo.Name))
                    cpy.MemberDatas.RemoveAt(i);
            }

            return cpy;
        }
    }
}
