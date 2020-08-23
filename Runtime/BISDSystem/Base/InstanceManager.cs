using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class InstanceManager : MonoBehaviour, IInstanceRegisterBehaviour, IInstanceProviderBehaviour, IInstanceProvider, IInstanceRegister
    {
        [ShowInInspector]
        private TypeLookup typeLookup;

        private TypeLookup TypeLookup
        {
            get
            {
                if(typeLookup == null)
                {
                    typeLookup = new TypeLookup();
                }
                return typeLookup;
            }
        }
        public IInstanceRegister InstanceRegister => this;

        public IInstanceProvider InstanceProvider => this;

        public T Get<T>()
        {
            return TypeLookup.Get<T>();
        }

        public void Register<T>(T instance)
        {
            TypeLookup.Add(instance);
        }
    }
}
