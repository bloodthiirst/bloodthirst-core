using Bloodthirst.BType;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class Matrix4x4PropertyFilter : IBJsonPropertyFilter<Matrix4x4>
    {
        internal static readonly List<string> allowedMembers = new List<string>()
        {
            nameof(Matrix4x4.m00),
            nameof(Matrix4x4.m01),
            nameof(Matrix4x4.m02),
            nameof(Matrix4x4.m03),

            nameof(Matrix4x4.m10),
            nameof(Matrix4x4.m11),
            nameof(Matrix4x4.m12),
            nameof(Matrix4x4.m13),

            nameof(Matrix4x4.m20),
            nameof(Matrix4x4.m21),
            nameof(Matrix4x4.m22),
            nameof(Matrix4x4.m23),

            nameof(Matrix4x4.m30),
            nameof(Matrix4x4.m31),
            nameof(Matrix4x4.m32),
            nameof(Matrix4x4.m33),

            nameof(Matrix4x4.identity),
            nameof(Matrix4x4.decomposeProjection),
            nameof(Matrix4x4.lossyScale),
            nameof(Matrix4x4.inverse),
            nameof(Matrix4x4.isIdentity),
            nameof(Matrix4x4.determinant),
            nameof(Matrix4x4.inverse),
            nameof(Matrix4x4.rotation),
            nameof(Matrix4x4.transpose),
            nameof(Matrix4x4.zero)
        };

        bool IBJsonPropertyFilter.CanFilter(Type t)
        {
            return t == typeof(Matrix4x4);
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
