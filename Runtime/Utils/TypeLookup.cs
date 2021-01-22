using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.Utils
{
    public class TypeLookup
    {
        [ShowInInspector]
        private Dictionary<Type, object> dictionary;

        public TypeLookup()
        {
            dictionary = new Dictionary<Type, object>();
        }

        public void Add<T>(T obj)
        {
            Type type = typeof(T);

            if (!dictionary.ContainsKey(type))
            {
                dictionary.Add(type, obj);
                return;
            }

            dictionary[type] = obj;
        }

        public T Get<T>()
        {
            Type type = typeof(T);

            if (!dictionary.ContainsKey(type))
            {
                return default;
            }

            return (T)dictionary[type];
        }

        public void Remove<T>(T obj)
        {
            Type type = typeof(T);

            dictionary.Remove(type);
        }

    }
}
