using Assets.Scripts.BISDSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BISDSystem
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

    }
}
