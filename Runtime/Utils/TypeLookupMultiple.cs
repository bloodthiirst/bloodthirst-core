using System;
using System.Collections.Generic;
using System.Linq;

namespace Bloodthirst.Core.Utils
{
    public class TypeLookupMultiple
    {
        private Dictionary<Type, List<object>> dictionary;

        public TypeLookupMultiple()
        {
            dictionary = new Dictionary<Type, List<object>>();
        }

        public void Add<T>(T obj)
        {
            Type type = typeof(T);

            if (!dictionary.ContainsKey(type))
            {
                dictionary.Add(type, new List<object>());
            }

            dictionary[type].Add(obj);
        }

        public IEnumerable<T> Get<T>()
        {
            Type type = typeof(T);

            if (!dictionary.ContainsKey(type))
            {
                dictionary.Add(type, new List<object>());
            }

            return dictionary[type].Cast<T>();
        }

        public void Remove<T>(T obj)
        {
            Type type = typeof(T);

            if (!dictionary.ContainsKey(type))
            {
                return;
            }

            dictionary[type].Remove(obj);
        }

    }
}
