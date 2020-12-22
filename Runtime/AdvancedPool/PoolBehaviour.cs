using UnityEngine;

namespace Bloodthirst.Core.AdvancedPool
{
    public abstract class PoolBehaviour<TObejct> : MonoBehaviour where TObejct : MonoBehaviour
    {
        [SerializeField]
        private Transform poolContainer = null;

        [SerializeField]
        private TObejct prefab;

        [SerializeField]
        private int poolCount;

        private Pool<TObejct> _Pool;
        public Pool<TObejct> Pool
        {
            get
            {
                if(_Pool == null)
                {
                    _Pool = new Pool<TObejct>(poolContainer , prefab , poolCount);
                }
                return _Pool;
            }
        }
    }
}
