﻿#if ODIN_INSPECTOR
	using Sirenix.Serialization;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.BProvider
{
    public class BProviderList<T> : IBProviderList
    {
        private Action<object> onAdded;
        private Action<object> onRemoved;
        event Action<object> IBProviderList.OnAdded
        {
            add
            {
                onAdded += value;
            }

            remove
            {
                onAdded -= value;
            }
        }
        event Action<object> IBProviderList.OnRemoved
        {
            add
            {
                onRemoved += value;
            }

            remove
            {
                onRemoved -= value;
            }
        }

        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;

        #if ODIN_INSPECTOR[OdinSerialize]#endif
        private List<T> elements;
        ICollection IBProviderList.Elements => elements;

        public BProviderList()
        {
            elements = new List<T>();
        }

        public void Add(T element)
        {
#if UNITY_EDITOR
            if(elements.Contains(element))
            {
                Debug.LogError($"Instance of {element} is already added");
            }
#endif

            elements.Add(element);
            onAdded?.Invoke(element);
            OnAdded?.Invoke(element);
        }

        public bool Remove(T element)
        {
            bool res = elements.Remove(element);

            if (res)
            {
                onRemoved?.Invoke(element);
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