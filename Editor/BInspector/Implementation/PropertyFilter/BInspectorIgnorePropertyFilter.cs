using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Editor.BInspector
{
    internal class BInspectorIgnorePropertyFilter : IBInspectorPropertyFilter
    {
        private static Type ignoreType = typeof(BInspectorIgnore);
        bool IBInspectorPropertyFilter.CanFilter(Type t)
        {
            return true;
        }

        BTypeData IBInspectorPropertyFilter.FilteredProperties (BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for(int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                if(curr.Attributes.ContainsKey(ignoreType))
                {
                    cpy.MemberDatas.RemoveAt(i);
                }
            }

            return cpy;
        }
    }
}
