using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.Singleton
{
    public abstract class UnitySingleton<TConcrete> : UnitySingleton<TConcrete, TConcrete> where TConcrete : UnitySingleton<TConcrete>
    {

    }

    public abstract class UnitySingleton<TConcrete , TInterface> : MonoBehaviour, ISetupSingletonPass
        where TConcrete : UnitySingleton<TConcrete , TInterface> , TInterface
        where TInterface : class
    {
        [ShowInInspector]
        [ReadOnly]
        private TConcrete instance;

        private TConcrete Instance
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

        protected virtual void Awake()
        {

            if (instance == null)
            {
                instance = GetComponent<TConcrete>();
                return;
            }

        }

        void ISetupSingletonPass.Execute()
        {
            BProviderRuntime.Instance.RegisterSingleton<TConcrete, TInterface>(Instance);
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
