using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierConcreteTypeComplex : BCopierBase
    {
        private Type Type { get; set; }

        internal override Type CopierType()
        {
            return Type;
        }

        private Func<object> Constructor { get; set; }

        #region members
        private int MemberCount { get; set; }
        private List<MemberInfo> MemberInfos { get; set; }
        private List<IBCopierInternal> Copiers { get; set; }
        private List<Func<object , object>> Getters { get; set; }
        private List<Action<object , object>> Setters { get; set; }
        #endregion

        internal BCopierConcreteTypeComplex(Type t)
        {
            Type = t;

            BDeepCopyProvider.Register(this);

            Initialize();
        }

        private void Initialize()
        {
            MemberInfos = GetMembers().ToList();

            Constructor = GetConstructor(Type);

            MemberCount = MemberInfos.Count;

            Copiers = new List<IBCopierInternal>();
            Getters = new List<Func<object, object>>();
            Setters = new List<Action<object, object>>();

            for (int i = 0; i < MemberCount; i++)
            {
                MemberInfo curr = MemberInfos[i];
                Getters.Add(GetMemberGetter(curr));
                Setters.Add(GetMemberSetter(curr));
                Copiers.Add(BDeepCopyProvider.GetOrCreate(GetUnderlyingType(curr)));
            }
        }

        private IEnumerable<MemberInfo> GetMembers()
        {
            PropertyInfo[] props = Type.GetProperties();

            FieldInfo[] fields = Type.GetFields();

            foreach(PropertyInfo p in props)
            {
                yield return p;
            }

            foreach(FieldInfo f in fields)
            {
                yield return f;
            }
        }

        internal override object CreateEmptyCopy(object original)
        {
            return Constructor();
        }

        internal override object Copy(object t, object instance , BCopierContext copierContext)
        {

            for(int i = 0; i < MemberCount; i++)
            {
                Action<object, object> setter = Setters[i];
                Func<object, object> getter = Getters[i];
                IBCopierInternal copier = Copiers[i];

                object originalVal = getter.Invoke(t);
                object copy = copier.Copy(originalVal , copierContext);

                // copy from original and assign to new instance
                setter.Invoke(instance, copy);
            }

            return instance;
        }

        // returns property getter
        internal Func<object, object> GetMemberGetter(MemberInfo propertyInfo)
        {
            ParameterExpression paramExpression = Expression.Parameter(typeof(object), "value");

            UnaryExpression castedInstance = Expression.Convert(paramExpression, propertyInfo.DeclaringType);

            Expression propertyGetterExpression = Expression.PropertyOrField(castedInstance, propertyInfo.Name);

            UnaryExpression castedResult = Expression.Convert(propertyGetterExpression, typeof(object));

            Func<object, object> result =
                Expression.Lambda<Func<object, object>>(castedResult, paramExpression)
                .Compile();

            return result;
        }

        // returns property setter:
        internal Action<object, object> GetMemberSetter(MemberInfo memberInfo)
        {
            // get the parameters
            ParameterExpression instance = Expression.Parameter(typeof(object) , "instance");
            ParameterExpression valueToSet = Expression.Parameter(typeof(object), memberInfo.Name);

            // cast
            UnaryExpression castedInstance = Expression.Convert(instance, memberInfo.DeclaringType);

            Type memberType = GetUnderlyingType(memberInfo);
            UnaryExpression castedValue = Expression.Convert(valueToSet, memberType);

            // get the prop
            MemberExpression propertyGetterExpression = Expression.PropertyOrField(castedInstance, memberInfo.Name);

            // assignment
            BinaryExpression assign = Expression.Assign(propertyGetterExpression, castedValue);

            Action<object, object> result = Expression.Lambda<Action<object, object>>
            (
                assign, instance, valueToSet
            )
            .Compile();

            return result;
        }

        private Type GetUnderlyingType(MemberInfo member)
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

        // returns property setter:
        internal Func<object> GetConstructor(Type type)
        {
            Func<object> result = Expression.Lambda<Func<object>>
            (
                Expression.New(type)
            )
            .Compile();

            return result;
        }
    }
}
