using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class BTypeData
    {
        public Type Type { get; private set; }
        public Func<object> Constructor { get; private set; }

        #region members
        private List<BMemberData> _MemberDatas { get; set; }

        public IReadOnlyList<BMemberData> MemberDatas => _MemberDatas;

        #endregion

        internal BTypeData(Type t)
        {
            Type = t;

            _MemberDatas = new List<BMemberData>();

            BTypeProvider.Register(this);

            Initialize();
        }

        protected void Initialize()
        {
            List<MemberInfo> MemberInfos = GetMembers().ToList();

            Constructor = ReflectionUtils.GetParameterlessConstructor(Type);

            for (int i = 0; i < MemberInfos.Count; i++)
            {
                MemberInfo curr = MemberInfos[i];

                // getter
                Func<object, object> getter = MemberGetter(curr);

                // setter
                Action<object, object> setter = MemberSetter(curr);

                // attr
                Dictionary<Type, Attribute> attrs = new Dictionary<Type, Attribute>();

                IEnumerable<Attribute> attrsList = curr.GetCustomAttributes(typeof(Attribute), true).Cast<Attribute>();

                foreach (CopierSettingAttribute a in attrsList)
                {
                    Type attrType = a.GetType();
                    attrs.Add(a.GetType(), a);
                }
                // create
                BMemberData memberData = new BMemberData();
                memberData.MemberInfo = curr;
                memberData.MemberGetter = getter;
                memberData.MemberSetter = setter;
                memberData.Attributes = attrs;

                // add
                _MemberDatas.Add(memberData);
            }
        }

        private IEnumerable<MemberInfo> GetMembers()
        {
            PropertyInfo[] props = Type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            FieldInfo[] fields = Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

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
