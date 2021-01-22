using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class PoolSingleton<TObejct> : MonoBehaviour where TObejct : MonoBehaviour
    {
        [SerializeField]
        private Transform poolContainer = null;

        [SerializeField]
        private TObejct prefab;

        [SerializeField]
        private int count;

        private static Pool<TObejct> pool;
        public static Pool<TObejct> Pool
        {
            get
            {
                if (pool == null)
                {
                    Transform t = Instance.poolContainer;
                    TObejct p = Instance.prefab;
                    int cnt = Instance.count;
                    pool = new Pool<TObejct>(t, p, cnt);
                }
                return pool;
            }
        }

        private static PoolSingleton<TObejct> instance;
        private static PoolSingleton<TObejct> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PoolSingleton<TObejct>>();
                }
                return instance;
            }
        }

        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
    }
}
