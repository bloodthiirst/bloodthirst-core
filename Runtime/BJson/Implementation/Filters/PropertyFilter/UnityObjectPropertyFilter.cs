using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    internal class UnityObjectPropertyFilter : IBJsonPropertyFilter<UnityEngine.Object>
    {
        internal static readonly List<string> IgnorableMembers = new List<string>()
        {
            "rigidbody",
            "rigidbody2D",
            "light",
            "animation",
            "renderer",
            "constantForce",
            "audio",
            "networkView",
            "collider",
            "collider2D",
            "hingeJoint",
            "particleSystem",
            "hideFlags",
            "name",
            "useGUILayout",
            "runInEditMode",
            "enabled",
            "gameObject",
            "allowPrefabModeInPlayMode",
            "isActiveAndEnabled",
            "transform",
            "tag"
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(UnityEngine.Object));
        }

        BTypeData IBJsonPropertyFilter.FilteredProperties (BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for(int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                if (IgnorableMembers.Contains(curr.MemberInfo.Name))
                    cpy.MemberDatas.RemoveAt(i);
            }

            return cpy;
        }
    }
}
