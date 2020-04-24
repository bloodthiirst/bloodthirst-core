using Assets.Scripts.BISDSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BISDSystem
{
    public struct EntityState<T> : IEntityState<T> where T : EntityData
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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (!(obj is EntityState<T>))
                return false;

            return ((EntityState<T>)obj).Id == Id;
        }
    }
}
