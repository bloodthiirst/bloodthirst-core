using Bloodthirst.Core.Utils;
using System;
using System.Reflection;

namespace Bloodthirst.System.Quest.Editor
{
    public class UnityObjectBindableUIFactory : IBindableUIFactory
    {
        public bool CanBind(MemberInfo memberInfo)
        {
            Type t = null;

            switch (memberInfo.MemberType)
            {
                case MemberTypes.All:
                case MemberTypes.Constructor:
                case MemberTypes.Custom:
                case MemberTypes.Event:
                case MemberTypes.NestedType:
                case MemberTypes.Field:
                    {
                        t = (memberInfo as FieldInfo).FieldType;
                        break;
                    }
                case MemberTypes.Property:
                    {
                        t = (memberInfo as PropertyInfo).PropertyType;
                        break;
                    }
                default:
                    break;
            }
            
            return TypeUtils.IsSubTypeOf(t , typeof(UnityEngine.Object));
        }

        public IBindableUI CreateUI()
        {
            return new UnityObjectBindableUI();
        }
    }
}
