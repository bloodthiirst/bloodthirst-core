using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    public class InstanceWatcher<T> : IDisposable
    {
        private static Type type = typeof(T);

        private static Type[] typeAsKeyArray = new Type[] { type };

        private List<IBProviderList> providers;

        public event Action<T> OnAdded;
        public event Action<T> OnRemoved;
        public event Action<T> OnChanged;

        internal InstanceWatcher(BProvider provider)
        {
            providers = new List<IBProviderList>();

            TreeLeaf<Type, IBProviderList> leaf = provider.ClassInstances.GetOrCreateLeaf(typeAsKeyArray);

            foreach (IBProviderList list in leaf.TraverseAllSubElements())
            {
                providers.Add(list);

                list.OnAdded += HandleOnAdd;
                list.OnRemoved += HandleOnRemove;
            }
        }

        private void HandleOnAdd(object obj)
        {
            OnAdded?.Invoke((T)obj);
            OnChanged?.Invoke((T)obj);
        }

        private void HandleOnRemove(object obj)
        {
            OnRemoved?.Invoke((T)obj);
            OnChanged?.Invoke((T)obj);
        }


        void IDisposable.Dispose()
        {
            foreach (IBProviderList provider in providers)
            {
                provider.OnAdded -= HandleOnAdd;
                provider.OnRemoved -= HandleOnRemove;
            }

            providers.Clear();
        }
    }


}