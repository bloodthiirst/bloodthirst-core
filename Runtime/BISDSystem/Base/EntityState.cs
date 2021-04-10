using UnityEngine;

namespace Bloodthirst.Core.BISDSystem
{
    public class EntityState<T> : IEntityState<T> where T : EntityData
    {

        [SerializeField]
        private T data;
        public T Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        [SerializeField]
        private int id;
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public virtual void PreloadStateFromData()
        {

        }
    }
}
