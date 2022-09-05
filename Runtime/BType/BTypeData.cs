using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace Bloodthirst.BType
{
    public class BTypeData
    {
        public static BTypeData Copy(BTypeData from)
        {
            return new BTypeData()
            {
                Constructor = from.Constructor,
                Type = from.Type,
                MemberDatas = new List<BMemberData>(from.MemberDatas)
            };
        }

        public Type Type { get; private set; }
        public Func<object> Constructor { get; private set; }

        #region members

        public List<BMemberData> MemberDatas { get; set; }

        #endregion

        private BTypeData()
        {

        }

        internal BTypeData(Type t)
        {
            Type = t;

            Assert.IsFalse(TypeUtils.PrimitiveTypes.Contains(t));
            Assert.IsFalse(t == typeof(string));

            MemberDatas = new List<BMemberData>();
            BTypeProvider.Register(this);
            Initialize();
        }

        protected void Initialize()
        {
            List<MemberInfo> MemberInfos = GetMembers().ToList();

            if (Type.IsValueType)
            {
                Constructor = ReflectionUtils.GetDefaultValue(Type);
            }
            else
            {
                Constructor = ReflectionUtils.GetParameterlessConstructor(Type);
            }

            for (int i = 0; i < MemberInfos.Count; i++)
            {
                MemberInfo curr = MemberInfos[i];

                // getter
                Func<object, object> getter = MemberGetter(curr);

                // setter
                Action<object, object> setter = MemberSetter(curr);

                // direct attr
                Dictionary<Type, Attribute> directAttrs = new Dictionary<Type, Attribute>();
                IEnumerable<Attribute> directAttrList = curr.GetCustomAttributes(typeof(Attribute), true).Cast<Attribute>();


                foreach (Attribute attributeValue in directAttrList)
                {
                    Type attrType = attributeValue.GetType();

                    if (!directAttrs.ContainsKey(attrType))
                    {
                        directAttrs.Add(attrType, attributeValue);
                    }
                }


                // inherited attr
                Dictionary<Type, List<Attribute>> inheritedAttrs = new Dictionary<Type, List<Attribute>>();
                List<Attribute> inheritedAttrList = curr.GetAttributesWithInheritence<Attribute>();

                foreach (Attribute a in inheritedAttrList)
                {
                    Type attrType = a.GetType();

                    if (!inheritedAttrs.TryGetValue(attrType, out var lst))
                    {
                        lst = new List<Attribute>();
                        inheritedAttrs.Add(attrType, lst);
                    }

                    lst.Add(a);
                }

                // create
                BMemberData memberData = new BMemberData();
                memberData.Type = ReflectionUtils.GetMemberType(curr);
                memberData.MemberInfo = curr;
                memberData.MemberGetter = getter;
                memberData.MemberSetter = setter;
                memberData.DirectAttributes = directAttrs;
                memberData.InheritedAttributes = inheritedAttrs;

                // add
                MemberDatas.Add(memberData);
            }
        }

        private IEnumerable<MemberInfo> GetMembers()
        {
            PropertyInfo[] props = Type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            FieldInfo[] fields = Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in props)
            {
                // to skip events
                if (TypeUtils.IsSubTypeOf(p.PropertyType, typeof(Delegate)))
                    continue;

                if (p.GetIndexParameters().Length != 0)
                    continue;

                if (p.CanRead && p.CanWrite)
                    yield return p;
            }

            foreach (FieldInfo f in fields)
            {
                // to skip events
                if (TypeUtils.IsSubTypeOf(f.FieldType, typeof(Delegate)))
                    continue;

                if (!f.Name.EndsWith("__BackingField"))
                    yield return f;
            }
        }

        /// <summary>
        /// returns property getter
        /// </summary>
        internal Func<object, object> MemberGetter(MemberInfo memberInfo)
        {
            Func<object, object> result = null;
#if false
            #region debug getter
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                result = (i) => field.GetValue(i);
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)memberInfo;
                result = (i) => prop.GetValue(i);
            }
            #endregion
#else
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                result = ReflectionUtils.EmitFieldGetter(field);
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)memberInfo;
                result = ReflectionUtils.EmitPropertyGetter(prop);
            }
#endif
            return result;
        }

        /// <summary>
        /// returns property setter
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        internal Action<object, object> MemberSetter(MemberInfo memberInfo)
        {
            Action<object, object> result = null;
#if false
            #region debug setter
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                result = (i, o) => field.SetValue(i, o);
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)memberInfo;
                result = (i, o) => prop.SetValue(i, o);
            }
            #endregion
#else
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)memberInfo;
                result = ReflectionUtils.EmitFieldSetter(field);
            }
            else if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)memberInfo;
                result = ReflectionUtils.EmitPropertySetter(prop);
            }
#endif
            return result;

        }

    }
}
