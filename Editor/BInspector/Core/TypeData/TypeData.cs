using Bloodthirst.Core.Utils;
using Bloodthirst.JsonUnityObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Editor.BInspector
{
    internal class TypeData
    {
        public Type Type { get; set; }
        public List<MemberData> MemberDatas { get; set; }

        public TypeData(Type type)
        {
            // type
            Type = type;

            MemberDatas = new List<MemberData>();

            // fields
            foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                MemberData f = new MemberData()
                {
                    MemberInfo = field,
                    Getter = ReflectionUtils.EmitFieldGetter(field),
                    Setter = ReflectionUtils.EmitFieldSetter(field)
                };

                MemberDatas.Add(f);
            }

            // properties
            foreach(PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                //
                if (JsonUnityObjectSettings.MonoBehaviourIgnorableMembers.Contains(property.Name))
                    continue;

                MemberData p = new MemberData()
                {
                    MemberInfo = property
                };

                if (property.CanRead)
                    p.Getter = ReflectionUtils.EmitPropertyGetter(property);

                if (property.CanWrite)
                    p.Setter = ReflectionUtils.EmitPropertySetter(property);

                MemberDatas.Add(p);
            }       
        }

    }

    public class MemberData
    {
        public MemberInfo MemberInfo { get; internal set; }
        public Func<object,object> Getter { get; internal set; }
        public Action<object,object> Setter { get; internal set; }
    }
}
