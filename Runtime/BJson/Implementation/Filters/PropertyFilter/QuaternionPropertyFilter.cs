using Bloodthirst.BType;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class QuaternionPropertyFilter : IBJsonPropertyFilter<Quaternion>
    {
        internal static readonly List<string> IgnorableMembers = new List<string>()
        {
            nameof(Quaternion.eulerAngles)
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return t == typeof(Quaternion);
        }

        BTypeData IBJsonPropertyFilter.FilteredProperties(BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for (int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                if (IgnorableMembers.Contains(curr.MemberInfo.Name))
                    cpy.MemberDatas.RemoveAt(i);
            }

            return cpy;
        }
    }
}
