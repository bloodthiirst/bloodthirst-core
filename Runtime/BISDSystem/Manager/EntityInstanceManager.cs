using Bloodthirst.Core.BProvider;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    [RequireComponent(typeof(EntityIdentifier))]
    public class EntityInstanceManager : MonoBehaviour,
        IEntityInstanceRegisterBehaviour, IEntityInstanceRegister,
        IEntityInstanceProviderBehaviour, IEntityInstanceProvider
    {
        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private BProvider.BProvider instanceProvider;

        public event Action<object> OnRegistered;

        public event Action<object> OnUnregistered;

        private BProvider.BProvider Provider
        {
            get
            {
                if (instanceProvider == null)
                {
                    instanceProvider = new BProvider.BProvider();
                }
                return instanceProvider;
            }
        }
        public IEntityInstanceRegister InstanceRegister => this;

        public IEntityInstanceProvider InstanceProvider => this;

        public T Get<T>() where T : class
        {
            return Provider.GetSingleton<T>();
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
