using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [RequireComponent(typeof(EntityIdentifier))]
    public class InstanceManager : MonoBehaviour, IInstanceRegisterBehaviour, IInstanceProviderBehaviour, IInstanceProvider, IEntityInstanceRegister
    {
        [ShowInInspector]
        private BProvider instanceProvider;

        public event Action<object> OnRegistered;

        public event Action<object> OnUnregistered;

        private BProvider Provider
        {
            get
            {
                if (instanceProvider == null)
                {
                    instanceProvider = new BProvider();
                }
                return instanceProvider;
            }
        }
        public IEntityInstanceRegister InstanceRegister => this;

        public IInstanceProvider InstanceProvider => this;

        public T Get<T>() where T : class
        {
            return Provider.GetSingleton<T>().Get;
        }

        public void Register<T>(T instance) where T : class
        {
            Provider.RegisterSingleton(instance);
            OnRegistered?.Invoke(instance);

        }

        public void Unregister<T>(T instance) where T : class
        {
            Provider.RemoveSingleton(instance);
            OnUnregistered?.Invoke(instance);
        }
    }
}
