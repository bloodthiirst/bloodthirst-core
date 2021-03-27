using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;

namespace Bloodthirst.Core.Pooling
{
    public class SerializedUnitySingleton<T> : SerializedMonoBehaviour, ISingletonPass where T : SerializedUnitySingleton<T>
    {

        protected static T instance;

        [ShowInInspector]
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

        public void DoSingletonPass()
        {
            if (instance == null)
            {
                instance = GetComponent<T>();
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
    }


}
