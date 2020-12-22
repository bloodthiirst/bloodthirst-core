using Sirenix.Serialization;
using System;
using UnityEngine;

namespace JsonDB
{
    public class DbReference<T> : IDbReference where T : IDbEntity
    {
        private static Type type = typeof(T);
        [SerializeField]
        private int? referenceId;
        public int? ReferenceId { get => referenceId; set => referenceId = value; }
        Type IDbReference.EntityType { get => type; }
        public IDbEntity Entity { get; set; }

        public DbReference()
        {
            ReferenceId = null;
        }

        public static implicit operator DbReference<T>(T right)
        {
            return new DbReference<T>()
            {
                ReferenceId = right.EntityId,
                Entity = right
            };
        }


        public static implicit operator DbReference<T>(int id)
        {
            return new DbReference<T>()
            {
                ReferenceId = id
            };
        }

        public static implicit operator T (DbReference<T> reference)
        {
            if (reference == null)
                return default;

            return (T) reference.Entity;
        }
    }
}
