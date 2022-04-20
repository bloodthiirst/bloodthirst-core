using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace Bloodthirst.Core.Utils
{
    public class ReflectionUtils
    {

        /// <summary>
        /// Recrsively goes through the objects to check for unused terms
        /// </summary>
        /// <param name="rootObject"></param>
        /// <param name="instance"></param>
        /// <param name="scannedObjs"></param>
        /// <param name="terms"></param>
        private static void RecursiveGetMissingTerms(object rootObject, object instance, HashSet<object> scannedObjs, List<FieldInstancePair> result, Func<object, FieldInfo, bool> fieldFilter)
        {
            // if we already scanned the instance
            // then we leave
            if (!scannedObjs.Add(instance))
                return;

            List<FieldInfo> allFields = instance.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            List<FieldInfo> leafFields = new List<FieldInfo>();

            // filter fields
            foreach (FieldInfo f in allFields)
            {
                if (fieldFilter(instance, f))
                {
                    result.Add(new FieldInstancePair() { Field = f, Instance = instance, RootInstance = rootObject });
                }
                else
                {
                    leafFields.Add(f);
                }
            }

            // recursivly go into the rest of the fields
            foreach (FieldInfo f in leafFields)
            {
                Type t = f.FieldType;

                if (t.IsPrimitive || t == typeof(decimal) || t == typeof(string))
                {
                    continue;
                }

                object fieldValue = f.GetValue(instance);

                if (fieldValue == null)
                    continue;

                RecursiveGetMissingTerms(rootObject, fieldValue, scannedObjs, result, fieldFilter);
            }
        }

        public struct FieldInstancePair
        {
            public object RootInstance { get; set; }
            public object Instance { get; set; }
            public FieldInfo Field { get; set; }
        }

        private static List<FieldInstancePair> SelectFields(List<object> objects, Func<object, FieldInfo, bool> fieldFilter)
        {
            HashSet<object> scannedObjs = new HashSet<object>();
            List<FieldInstancePair> res = new List<FieldInstancePair>();

            foreach (object o in objects)
            {
                RecursiveGetMissingTerms(o, o, scannedObjs, res, fieldFilter);
            }

            return res;
        }

        public static IEnumerable<MemberInfo> GetFieldsAndProperties(Type t)
        {
            PropertyInfo[] props = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            FieldInfo[] fields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);


            foreach (PropertyInfo p in props)
            {
                if (p.CanRead && p.CanWrite)
                    yield return p;
            }

            foreach (FieldInfo f in fields)
            {
                if (!f.Name.EndsWith("__BackingField"))
                    yield return f;
            }
        }

        /// <summary>
        /// Get the type of the member if it's a field or a property
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type GetMemberType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        #region Expression

        public static Func<object> GetDefaultValue(Type type)
        {
            // Validate parameters.
            if (type == null) throw new ArgumentNullException("type");

            try
            {
                // We want an Func<object> which returns the default.
                // Create that expression here.
                Expression<Func<object>> e = Expression.Lambda<Func<object>>(
                // Have to convert to object.
                Expression.Convert(
                    // The default value, always get what the *code* tells us.
                    Expression.Default(type), typeof(object)
                )
            );

                // Compile and return the value.
                return e.Compile();
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// Get a delegate that returns a new instance of type <paramref name="type"/> using it's parameterless constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<object> GetParameterlessConstructor(Type type)
        {
            ConstructorInfo parameterlessCtor = type.GetConstructor(Type.EmptyTypes);

            if (parameterlessCtor == null)
                return null;

            Func<object> result = Expression.Lambda<Func<object>>
            (
                Expression.Convert(Expression.New(type), typeof(object))
            )
            .Compile();
            return result;

        }

        /// <summary>
        /// <para>Create a delegate using the Expressing API that behaves as a setter for a member</para>
        /// <para>The member has to be public and able to be written into</para>
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Action<object, object> ExpressionMemberSetter(MemberInfo memberInfo)
        {
            Action<object, object> result = null;

            // get the parameters
            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression valueToSet = Expression.Parameter(typeof(object), memberInfo.Name);

            // cast
            UnaryExpression castedInstance = Expression.Convert(instance, memberInfo.DeclaringType);

            Type memberType = GetMemberType(memberInfo);
            UnaryExpression castedValue = Expression.Convert(valueToSet, memberType);

            // get the prop
            MemberExpression propertyGetterExpression = Expression.PropertyOrField(castedInstance, memberInfo.Name);

            // assignment
            BinaryExpression assign = Expression.Assign(propertyGetterExpression, castedValue);

            result = Expression.Lambda<Action<object, object>>
            (
                assign, instance, valueToSet
            )
            .Compile();

            return result;
        }

        /// <summary>
        /// <para>Create a delegate using the Expressing API that behaves as a getter for a member</para>
        /// <para>The member has to be public and able to be read</para>
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Func<object, object> ExpressionMemberGetter(MemberInfo memberInfo)
        {
            Func<object, object> result = null;

            ParameterExpression paramExpression = Expression.Parameter(typeof(object), "value");

            UnaryExpression castedInstance = Expression.Convert(paramExpression, memberInfo.DeclaringType);

            Expression propertyGetterExpression = Expression.PropertyOrField(castedInstance, memberInfo.Name);

            UnaryExpression castedResult = Expression.Convert(propertyGetterExpression, typeof(object));

            result =
                Expression.Lambda<Func<object, object>>(castedResult, paramExpression)
                .Compile();

            return result;
        }
        #endregion

        #region emit IL

        /// <summary>
        /// <para>Create a delegate using the ILGenerator API that behaves as a setter for a property</para>
        /// <para>The property has to be public and able to read and written into</para>
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        /// THIS IS THE ISSUE

        public static Action<object, object> EmitPropertySetter(PropertyInfo prop)
        {
            try
            {
                string methodName = prop.ReflectedType.FullName + "set_prop_" + prop.Name;
                DynamicMethod setter = new DynamicMethod(methodName, null, new Type[2] { typeof(object), typeof(object) }, true);
                ILGenerator gen = setter.GetILGenerator();

                // load the instance to the stack
                gen.Emit(OpCodes.Ldarg_0);

                // cast it to the proper type
                // if struct
                if (prop.DeclaringType.IsValueType)
                {
                    gen.Emit(OpCodes.Unbox, prop.DeclaringType);
                }

                // if class
                else
                {
                    gen.Emit(OpCodes.Castclass, prop.DeclaringType);
                }

                // load the parameter to the stack
                gen.Emit(OpCodes.Ldarg_1);

                // inverstigate differnec between unbox and unbox_any
                // answer :
                // - unbox      : gets the pointer to the struct and pushes it into the stack
                // - unbox_any  : gets the pointer to the struct , loads the value , THEN pushes the value into the stack
                // since we're gonna use this in the "setMethod" , we're gonna need the value and NOT the pointer
                // so we use unbox_any
                // ps : if the value in the stack in a reference type , then "unbox_any" acts as "castClass"
                gen.Emit(OpCodes.Unbox_Any, prop.PropertyType);


                // call the set function
                if (prop.DeclaringType.IsValueType)
                {
                    gen.Emit(OpCodes.Call, prop.SetMethod);
                }
                else
                {
                    gen.Emit(OpCodes.Callvirt, prop.SetMethod);
                }


                gen.Emit(OpCodes.Ret);


                Delegate del = setter.CreateDelegate(typeof(Action<object, object>));
                Action<object, object> casted = (Action<object, object>)del;
                return casted;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// <para>Create a delegate using the Expressing API that behaves as a getter for a property</para>
        /// <para>The property has to be public and able to be read</para>
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Func<object, object> EmitPropertyGetter(PropertyInfo prop)
        {
            try
            {
                string methodName = "get_prop_" + prop.Name;
                DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(object), new Type[1] { typeof(object) }, true);
                ILGenerator gen = getterMethod.GetILGenerator();

                // load the instance to the stack
                gen.Emit(OpCodes.Ldarg_0);

                // cast it to the proper type
                // if struct
                if (prop.DeclaringType.IsValueType)
                {
                    gen.Emit(OpCodes.Unbox, prop.DeclaringType);
                    gen.Emit(OpCodes.Call, prop.GetMethod);
                }

                // if class
                else
                {
                    gen.Emit(OpCodes.Castclass, prop.DeclaringType);
                    gen.Emit(OpCodes.Callvirt, prop.GetMethod);
                }

                // the value needs to be boxed before it's returned
                // since we return it as "object"
                if (prop.PropertyType.IsValueType)
                {
                    gen.Emit(OpCodes.Box, prop.PropertyType);
                }

                gen.Emit(OpCodes.Ret);

                Delegate del = getterMethod.CreateDelegate(typeof(Func<object, object>));
                Func<object, object> casted = (Func<object, object>)del;
                return casted;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        /// <summary>
        /// <para>Create a delegate using the Expressing API that behaves as a getter for a field</para>
        /// <para>The field has to be public and able to be read</para>
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Func<object, object> EmitFieldGetter(FieldInfo field)
        {
            string methodName = "get_prop_" + field.Name;
            DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(object), new Type[1] { typeof(object) }, true);
            ILGenerator gen = getterMethod.GetILGenerator();

            // load the instance to the stack
            gen.Emit(OpCodes.Ldarg_0);

            // cast it to the proper type
            // if struct
            if (field.DeclaringType.IsValueType)
            {
                gen.Emit(OpCodes.Unbox, field.DeclaringType);
                gen.Emit(OpCodes.Ldfld, field);
            }

            // if class
            else
            {
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
                gen.Emit(OpCodes.Ldfld, field);
            }

            // the value needs to be boxed before it's returned
            // since we return it as "object"
            if (field.FieldType.IsValueType)
            {
                gen.Emit(OpCodes.Box, field.FieldType);
            }

            gen.Emit(OpCodes.Ret);

            Delegate del = null;

            try
            {
                del = getterMethod.CreateDelegate(typeof(Func<object, object>));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            Func<object, object> casted = (Func<object, object>)del;
            return casted;
        }


        /// <summary>
        /// <para>Create a delegate using the ILGenerator API that behaves as a setter for a field</para>
        /// <para>The field has to be public and able to read and written into</para>
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Action<object, object> EmitFieldSetter(FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + "set_prop_" + field.Name;
            DynamicMethod setter = new DynamicMethod(methodName, null, new Type[2] { typeof(object), typeof(object) }, true);
            ILGenerator gen = setter.GetILGenerator();

            // load the instance to the stack
            gen.Emit(OpCodes.Ldarg_0);

            // cast it to the proper type
            // if struct
            if (field.DeclaringType.IsValueType)
            {
                gen.Emit(OpCodes.Unbox, field.DeclaringType);
            }

            // if class
            else
            {
                gen.Emit(OpCodes.Castclass, field.DeclaringType);
            }

            // load the parameter to the stack
            gen.Emit(OpCodes.Ldarg_1);

            // inverstigate differnec between unbox and unbox_any
            // answer :
            // - unbox      : gets the pointer to the struct and pushes it into the stack
            // - unbox_any  : gets the pointer to the struct , loads the value , THEN pushes the value into the stack
            // since we're gonna use this in the "setMethod" , we're gonna need the value and NOT the pointer
            // so we use unbox_any
            // ps : if the value in the stack in a reference type , then "unbox_any" acts as "castClass"
            gen.Emit(OpCodes.Unbox_Any, field.FieldType);


            // call the set function
            if (field.DeclaringType.IsValueType)
            {
                gen.Emit(OpCodes.Stfld, field);
            }
            else
            {
                gen.Emit(OpCodes.Stfld, field);
            }


            gen.Emit(OpCodes.Ret);


            Delegate del = setter.CreateDelegate(typeof(Action<object, object>));
            Action<object, object> casted = (Action<object, object>)del;
            return casted;
        }


        #endregion
    }
}
