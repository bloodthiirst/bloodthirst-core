using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Func<object, object>> Getters { get; set; }
        private List<Action<object, object>> Setters { get; set; }
        private List<Dictionary<Type, CopierSettingAttribute>> BCopierOverrides { get; set; }

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

            Constructor = ReflectionUtils.GetParameterlessConstructor(Type);

            MemberCount = MemberInfos.Count;

            Copiers = new List<IBCopierInternal>();
            Getters = new List<Func<object, object>>();
            Setters = new List<Action<object, object>>();
            BCopierOverrides = new List<Dictionary<Type, CopierSettingAttribute>>();

            for (int i = 0; i < MemberCount; i++)
            {
                MemberInfo curr = MemberInfos[i];
                Getters.Add(MemberGetter(curr));
                Setters.Add(MemberSetter(curr));
                Copiers.Add(BDeepCopyProvider.GetOrCreate(ReflectionUtils.GetMemberType(curr)));

                Dictionary<Type, CopierSettingAttribute> attrs = new Dictionary<Type, CopierSettingAttribute>();
                IEnumerable<CopierSettingAttribute> attrsList = curr.GetCustomAttributes<CopierSettingAttribute>(true);

                foreach (CopierSettingAttribute a in attrsList)
                {
                    attrs.Add(a.GetType(), a);
                }

                BCopierOverrides.Add(attrs);
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

        public override IReadOnlyList<MemberInfo> CopiableMembers()
        {
            return MemberInfos;
        }

        internal override object CreateEmptyCopy(object original)
        {
            return Constructor();
        }

        internal override object Copy(object t, object emptyCopy, BCopierContext copierContext, BCopierSettings copierSettings)
        {
            for (int i = 0; i < MemberCount; i++)
            {
#if DEBUG
                MemberInfo mem = MemberInfos[i];
#endif
                Func<object, object> getter = Getters[i];
                object originalVal = getter.Invoke(t);

                IBCopierInternal copier = Copiers[i];
                Dictionary<Type, CopierSettingAttribute> overrides = BCopierOverrides[i];

                object copy = null;


                Action<object, object> setter = Setters[i];
                if (copierSettings == null)
                {
                    copy = copier.Copy(originalVal, copierContext, copierSettings);
                }
                else
                {
                    // look for overrides
                    foreach (KeyValuePair<Type, CopierSettingAttribute> kv in overrides)
                    {
                        for (int c = 0; c < copierSettings.CopierOverrides.Count; c++)
                        {
                            IBCopierOverride curr = copierSettings.CopierOverrides[c];

                            if (curr.AttributeType == kv.Key)
                            {
                                copy = curr.CopyOverride(in originalVal, MemberInfos[i], kv.Value);
                                break;
                            }
                        }
                    }

                    // copy by default
                    if (copy == null)
                    {
                        copy = copier.Copy(originalVal, copierContext, copierSettings);
                    }
                }

                // copy from original and assign to new instance
                setter.Invoke(emptyCopy, copy);
            }

            return emptyCopy;
        }




        // returns property getter
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
            else if(memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo prop = (PropertyInfo)memberInfo;
                result = ReflectionUtils.EmitPropertyGetter(prop);
            }
#endif
            return result;
        }

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
