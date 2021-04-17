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

        private Pool<TObejct> pool;
        public Pool<TObejct> Pool
        {
            get
            {
                if (pool == null)
                {
                    TObejct p = prefab;
                    int cnt = count;
                    pool = new Pool<TObejct>(poolContainer, p, cnt);
                }
                return pool;
            }
        }
    }
}
