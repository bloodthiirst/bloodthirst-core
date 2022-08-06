using System;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    public class BProviderList<T> : IBProviderList
    {
        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;

        private List<T> elements;
        ICollection IBProviderList.Elements => elements;

        public BProviderList()
        {
            elements = new List<T>();
        }

        public void Add(T element)
        {
            elements.Add(element);
            OnAdded?.Invoke(element);
        }

        public bool Remove(T element)
        {
            bool res = elements.Remove(element);
            
            if (res)
            {
                OnRemoved?.Invoke(element);
            }

            return res;
        }

        void IBProviderList.Add(object element)
        {
            Add((T)element);
        }

        bool IBProviderList.Remove(object element)
        {
            return Remove((T)element);
        }
    }
}