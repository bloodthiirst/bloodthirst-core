using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.Singleton
{
    public abstract class UnitySingleton<TConcrete> : UnitySingleton<TConcrete, TConcrete> where TConcrete : UnitySingleton<TConcrete>
    {
        internal override void Register()
        {
            BProviderRuntime.Instance.RegisterSingleton<TConcrete, TConcrete>(Instance);
        }
    }

    public abstract class UnitySingleton<TConcrete , TInterface> : MonoBehaviour, ISetupSingletonPass
        where TConcrete : UnitySingleton<TConcrete , TInterface> , TInterface
        where TInterface : class
    {
        [ShowInInspector]
        [ReadOnly]
        private TConcrete instance;

        protected TConcrete Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GetComponent<TConcrete>();
                }

                if (instance == null)
                {
                    instance = FindObjectOfType<TConcrete>();
                }

                return instance;
            }
        }

        internal virtual void Register()
        {
            BProviderRuntime.Instance.RegisterSingleton<TConcrete, TConcrete>(Instance);
            BProviderRuntime.Instance.RegisterSingleton<TConcrete, TInterface>(Instance);
        }

        void ISetupSingletonPass.Execute()
        {
            Register();
            OnSetupSingletonPass();
        }

        protected virtual void OnSetupSingletonPass() { }

        private void OnEnable()
        {
            if (instance == null)
            {
                instance = GetComponent<TConcrete>();
                return;
            }
        }

    }


}
