using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.Singleton
{
    public abstract class BSingleton<TConcrete> : BSingleton<TConcrete, TConcrete> where TConcrete : BSingleton<TConcrete>
    {

    }

    public abstract class BSingleton<TConcrete, TInterface> : MonoBehaviour,
        IBSingleton
        where TConcrete : BSingleton<TConcrete, TInterface>, TInterface
        where TInterface : class
    {
        Type IBSingleton.Concrete => typeof(TConcrete);
        Type IBSingleton.Interface => typeof(TInterface);

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

        public virtual void OnSetupSingleton() { }

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
