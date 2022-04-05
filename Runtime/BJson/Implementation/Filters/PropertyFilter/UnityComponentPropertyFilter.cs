using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class UnityComponentPropertyFilter : IBJsonPropertyFilter<Component>
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
            return TypeUtils.IsSubTypeOf(t, typeof(Component));
        }

        BTypeData IBJsonPropertyFilter.FilteredProperties (BTypeData typeData)
        {
            BTypeData cpy = BTypeData.Copy(typeData);

            for(int i = cpy.MemberDatas.Count - 1; i >= 0; i--)
            {
                BMemberData curr = cpy.MemberDatas[i];

                // remove all "obsolete" unity props
                if (IgnorableMembers.Contains(curr.MemberInfo.Name))
                {
                    cpy.MemberDatas.RemoveAt(i);
                }

                // remove all the serializable "unity component" refs , since unity's serializer will do the job for us
                if( TypeUtils.IsSubTypeOf(curr.Type , typeof(Component)) && curr.MemberInfo.GetCustomAttributes<SerializeField>().Any())
                {
                    cpy.MemberDatas.RemoveAt(i);
                }
            }

            return cpy;
        }
    }
}
