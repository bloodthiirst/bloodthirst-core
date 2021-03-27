using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class InstanceManager : MonoBehaviour, IInstanceRegisterBehaviour, IInstanceProviderBehaviour, IInstanceProvider, IEntityInstanceRegister
    {
        [ShowInInspector]
        private TypeLookup typeLookup;

        public event Action<object> OnRegistered;

        public event Action<object> OnUnregistered;

        private TypeLookup TypeLookup
        {
            get
            {
                if (typeLookup == null)
                {
                    typeLookup = new TypeLookup();
                }
                return typeLookup;
            }
        }
        public IEntityInstanceRegister InstanceRegister => this;

        public IInstanceProvider InstanceProvider => this;

        public T Get<T>()
        {
            return TypeLookup.Get<T>();
        }

        public void Register<T>(T instance)
        {
            TypeLookup.Add(instance);
            OnRegistered?.Invoke(instance);

        }

        public void Unregister<T>(T instance)
        {
            TypeLookup.Remove(instance);
            OnUnregistered?.Invoke(instance);
        }
    }
}
