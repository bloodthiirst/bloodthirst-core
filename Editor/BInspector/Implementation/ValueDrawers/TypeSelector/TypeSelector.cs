using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // if type is a "constructed" generic type
            // so an already implemented generic type
            // like List<int> , and not List<T>
            if (!type.IsAbstract && !type.IsInterface)
            {
                // all generic types
                if (!type.IsGenericType && type.IsConstructedGenericType)
                {
                    result.Add(type);

                }
            }

            if (type.IsGenericType && type.IsInterface)
            {
                Type blankGenericType = type.GetGenericTypeDefinition();
                Type genericArgument = type.GetGenericArguments()[0];

                List<Type> genericTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsGenericType)
                .Where(t =>
                {
                    IEnumerable<Type> allBlankGenericInterfaces = t.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.IsGenericTypeDefinition ? i : i.GetGenericTypeDefinition());
                    return allBlankGenericInterfaces.Contains(blankGenericType);
                })
                .ToList();

                result.AddRange(genericTypes);
            }
            /*
            // generic type
            if(type.IsGenericType)
            {
                Type[] genericArgs = type.GetGenericArguments();

                // foreach type arg , we try to figure out it's constraints
                for (int i = 0; i < genericArgs.Length; i++)
                {
                    Type genericParam = genericArgs[i];

                    if (!genericArgs[i].IsGenericParameter)
                        continue;

                    Type[] typeConstraints = genericParam.GetGenericParameterConstraints();

                    if (typeConstraints.Length == 0)
                    {
                        Debug.Log($"The generic param {i} of {TypeUtils.GetNiceName(type)} has no type constraints");
                        continue;
                    }

                    foreach(Type c in typeConstraints)
                    {
                        if(c.IsInterface)
                        {
                            Debug.Log($"The generic param {i} of {TypeUtils.GetNiceName(type)} has to implements the interface {TypeUtils.GetNiceName(c)}");
                            continue;
                        }

                        if (c.IsClass)
                        {
                            Debug.Log($"The generic param {i} of {TypeUtils.GetNiceName(type)} has to implements as base class {TypeUtils.GetNiceName(c)}");
                            continue;
                        }
                    }

                    GenericParameterAttributes sConstraints = type.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

                    if (GenericParameterAttributes.None != (sConstraints & GenericParameterAttributes.DefaultConstructorConstraint))
                    {
                        Debug.Log($"The generic param {i} of {TypeUtils.GetNiceName(type)} must have a parameterless constructor.");
                    }
                    if (GenericParameterAttributes.None != (sConstraints & GenericParameterAttributes.ReferenceTypeConstraint))
                    {
                        Debug.Log($"The generic param {i} of {TypeUtils.GetNiceName(type)} must be a reference type.");
                    }
                    if (GenericParameterAttributes.None != (sConstraints & GenericParameterAttributes.NotNullableValueTypeConstraint))
                    {
                        Debug.Log($"The generic param {i} of {TypeUtils.GetNiceName(type)} must be a non-nullable value type.");
                    }

                }
            }
            */
            result.AddRange(normalTypes);

            // add null
            result.Insert(0, null);

            return result;
        }
    }
}