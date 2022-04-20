using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using TMPro;

namespace Bloodthirst.BJson
{
    internal class BJsonTMP_TextPropertyFilter : IBJsonPropertyFilter<TMP_Text>
    {
        internal static readonly List<string> allowedMembers = new List<string>()
        {
            nameof(TMP_Text.text),
            nameof(TMP_Text.fontStyle),
            nameof(TMP_Text.fontSize),
            nameof(TMP_Text.alignment)
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(TMP_Text));
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
