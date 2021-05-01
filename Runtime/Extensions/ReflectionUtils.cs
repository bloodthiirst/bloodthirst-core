﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Bloodthirst.Core.Utils
{
    public class ReflectionUtils
    {
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
        public static Func<object> GetParameterlessConstructor(Type type)
        {
            Func<object> result = Expression.Lambda<Func<object>>
            (
                Expression.New(type)
            )
            .Compile();

            return result;
        }

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

        public static Action<object, object> EmitPropertySetter(PropertyInfo prop)
        {
            string methodName = prop.ReflectedType.FullName + ".set_prop_" + prop.Name;
            DynamicMethod getterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(object), typeof(object)}, true) ;
            ILGenerator gen = getterMethod.GetILGenerator();



            gen.Emit(OpCodes.Ldarg_0);
            // TODO : check if works for structs too
            gen.Emit(OpCodes.Castclass, prop.DeclaringType);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, prop.PropertyType);
            gen.Emit(OpCodes.Callvirt, prop.SetMethod);
            gen.Emit(OpCodes.Ret);

            Delegate del = getterMethod.CreateDelegate(typeof(Action<object, object>));
            Action<object, object> casted = (Action<object, object>)del;
            return casted;
        }


        public static Func<object, object> EmitPropertyGetter(PropertyInfo prop)
        {
            string methodName = prop.ReflectedType.FullName + ".get_prop_" + prop.Name;
            DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(object), new Type[1] { typeof(object) }, true);
            ILGenerator gen = getterMethod.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            // TODO : check if works for structs too
            gen.Emit(OpCodes.Castclass, prop.DeclaringType);
            gen.Emit(OpCodes.Callvirt, prop.GetMethod);
            gen.Emit(OpCodes.Box, prop.PropertyType);
            gen.Emit(OpCodes.Ret);

            Delegate del = getterMethod.CreateDelegate(typeof(Func<object, object>));
            Func<object, object> casted = (Func<object, object>)del;
            return casted;
        }


        public static Func<object, object> EmitFieldGetter(FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + ".get_field_" + field.Name;
            DynamicMethod getterMethod = new DynamicMethod(methodName, typeof(object), new Type[1] { typeof(object) }, true);
            ILGenerator gen = getterMethod.GetILGenerator();



            gen.Emit(OpCodes.Ldarg_0);
            // TODO : check if works for structs too
            gen.Emit(OpCodes.Castclass, field.DeclaringType);
            gen.Emit(OpCodes.Ldfld, field);
            if (field.FieldType.IsValueType)
            {
                gen.Emit(OpCodes.Box, field.FieldType);
            }
            gen.Emit(OpCodes.Ret);

            Delegate del = getterMethod.CreateDelegate(typeof(Func<object, object>));
            Func<object, object> casted = (Func<object, object>)del;
            return casted;
        }

        public static Action<object, object> EmitFieldSetter(FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + ".set_field_" + field.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(object), typeof(object) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Castclass, field.DeclaringType);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Unbox_Any, field.FieldType);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);
            Delegate del = setterMethod.CreateDelegate(typeof(Action<object, object>));
            Action<object, object> casted = (Action<object, object>)del;
            return casted;
        }

        #endregion
    }
}
