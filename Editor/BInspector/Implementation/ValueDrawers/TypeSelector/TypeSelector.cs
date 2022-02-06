using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class TypeSelector : PopupField<Type>
    {
        public TypeSelector(Type t) : base(GetTypesList(t), 0, TypeToName, TypeToName)
        {
        }

        private static string TypeToName(Type type)
        {
            if (type == null)
                return "Null";

            return TypeUtils.GetNiceName(type);
        }

        private static List<Type> GetTypesList(Type type)
        {
            List<Type> result = new List<Type>();

            // all normal types
            List<Type> normalTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, type))
                .ToList();

            // generic type
            // mainly for collections
            if (!type.IsAbstract && !type.IsInterface)
            {
                // all generic types
                if (type.IsGenericType && type.IsConstructedGenericType)
                {
                    result.Add(type);

                }
            }

            result.AddRange(normalTypes);

            // add null
            result.Insert(0, null);

            return result;
        }
    }
}