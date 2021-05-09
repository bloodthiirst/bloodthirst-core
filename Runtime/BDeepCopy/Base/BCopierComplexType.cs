using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierComplexType : BCopierBase
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

        /// TODO : pre-cache the global overrides
        /// if there's one , then we return its result
        /// else we search the custom passed overrides
        /// else we do the default copier
        private List<Dictionary<Type, CopierSettingAttribute>> BCopierOverrides { get; set; }
        private List<List<Tuple<CopierSettingAttribute, IBCopierOverrideInternal>>> InternalBCopierOverrides { get; set; }

        #endregion

        internal BCopierComplexType(Type t)
        {
            Type = t;

            BDeepCopyProvider.Register(this);

            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();

            MemberInfos = GetMembers().ToList();

            Constructor = ReflectionUtils.GetParameterlessConstructor(Type);

            MemberCount = MemberInfos.Count;

            Copiers = new List<IBCopierInternal>();
            Getters = new List<Func<object, object>>();
            Setters = new List<Action<object, object>>();

            // custom
            BCopierOverrides = new List<Dictionary<Type, CopierSettingAttribute>>();

            // internal
            InternalBCopierOverrides = new List<List<Tuple<CopierSettingAttribute, IBCopierOverrideInternal>>>();

            for (int i = 0; i < MemberCount; i++)
            {
                MemberInfo curr = MemberInfos[i];
                Getters.Add(MemberGetter(curr));
                Setters.Add(MemberSetter(curr));
                Copiers.Add(BDeepCopyProvider.GetOrCreate(ReflectionUtils.GetMemberType(curr)));

                Dictionary<Type, CopierSettingAttribute> customAttrs = new Dictionary<Type, CopierSettingAttribute>();
                List<Tuple<CopierSettingAttribute, IBCopierOverrideInternal>> internalAttrs = new List<Tuple<CopierSettingAttribute, IBCopierOverrideInternal>>();

                IEnumerable<CopierSettingAttribute> attrsList = curr.GetCustomAttributes<CopierSettingAttribute>(true);

                foreach (CopierSettingAttribute a in attrsList)
                {
                    // check if internal
                    if (BCopierSettings.TypeToCopier.TryGetValue(a.GetType(), out IBCopierOverrideInternal copier))
                    {
                        internalAttrs.Add(new Tuple<CopierSettingAttribute, IBCopierOverrideInternal>(a, copier));
                    }

                    // else add to custom
                    else
                    {
                        customAttrs.Add(a.GetType(), a);
                    }
                }

                BCopierOverrides.Add(customAttrs);
                InternalBCopierOverrides.Add(internalAttrs);
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

        internal override object CreateEmptyCopy(object original)
        {
            return Constructor();
        }

        internal override object Copy(object t, object emptyCopy, BCopierContext copierContext, BCopierSettings copierSettings)
        {
            for (int i = 0; i < MemberCount; i++)
            {
                MemberInfo mem = MemberInfos[i];
                Func<object, object> getter = Getters[i];
                Action<object, object> setter = Setters[i];
                IBCopierInternal copier = Copiers[i];
                Dictionary<Type, CopierSettingAttribute> customOverrides = BCopierOverrides[i];
                List<Tuple<CopierSettingAttribute, IBCopierOverrideInternal>> internalOverrides = InternalBCopierOverrides[i];

                object originalVal = getter.Invoke(t);
                object copy = null;

                // if we found an internal override
                // then we apply it and go next
                if (TryInternalOverride(internalOverrides, mem, ref originalVal, copier, out copy))
                {
                    setter.Invoke(emptyCopy, copy);
                    continue;
                }

                // if no copier setting we use default
                if (copierSettings == null)
                {
                    copy = copier.Copy(originalVal, copierContext, copierSettings);
                }

                // try to check if we can apply an override
                // if not , then we use the default copy
                else if (!TryCustomOverride(copierSettings, mem, ref originalVal, copier, customOverrides, out copy))
                {
                    copy = copier.Copy(originalVal, copierContext, copierSettings);
                }

                // copy from original and assign to new instance
                setter.Invoke(emptyCopy, copy);
            }

            return emptyCopy;
        }

        private bool TryInternalOverride(List<Tuple<CopierSettingAttribute, IBCopierOverrideInternal>> internalOverrides, MemberInfo memberInfo, ref object originalVal, IBCopierInternal copier, out object copy)
        {
            // look for overrides
            for (int i = 0; i < internalOverrides.Count; i++)
            {
                Tuple<CopierSettingAttribute, IBCopierOverrideInternal> t = internalOverrides[i];

                copy = t.Item2.CopyOverride(originalVal, memberInfo, t.Item1, copier);
                return true;
            }

            copy = null;
            return false;
        }


        private bool TryCustomOverride(BCopierSettings copierSettings, MemberInfo memberInfo, ref object originalVal, IBCopierInternal copier, Dictionary<Type, CopierSettingAttribute> overrides, out object copy)
        {
            // look for overrides
            foreach (KeyValuePair<Type, CopierSettingAttribute> kv in overrides)
            {
                // custom copier overrides
                for (int c = 0; c < copierSettings.CopierOverrides.Count; c++)
                {
                    IBCopierOverride curr = copierSettings.CopierOverrides[c];

                    if (curr.AttributeType == kv.Key)
                    {
                        copy = curr.CopyOverride(in originalVal, memberInfo, kv.Value);
                        return true;
                    }
                }
            }

            copy = null;
            return false;
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
