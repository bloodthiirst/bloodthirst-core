using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.Singleton
{
    public abstract class UnitySingleton<T> : MonoBehaviour, ISetupSingletonPass where T : UnitySingleton<T>
    {
        [ShowInInspector]
        [ReadOnly]
        private T instance;

        private T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GetComponent<T>();
                }

                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {

            if (instance == null)
            {
                instance = GetComponent<T>();
                return;
            }

        }

        void ISetupSingletonPass.Execute()
        {
            BProviderRuntime.Instance.RegisterSingleton(Instance);
            OnSetupSingletonPass();
        }

        protected virtual void OnSetupSingletonPass() { }

        private void OnEnable()
        {
            if (instance == null)
            {
                instance = GetComponent<T>();
                return;
            }
        }

    }


}
