using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.Singleton
{
    public class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
    {

        [ShowInInspector]
        [ReadOnly]
        protected static T instance;

        public static T Instance
        {

            get
            {
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
