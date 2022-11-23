using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.BProvider
{
    public class SingletonWatcher<T> : IDisposable
    {
        private static Type type = typeof(T);

        private static Type[] typeAsKeyArray = new Type[] { type };

        private List<IBProviderSingleton> providers;

        public event Action<T,T> OnChanged;

        internal SingletonWatcher(BProvider provider)
        {
            providers = new List<IBProviderSingleton>();

            TreeLeaf<Type, IBProviderSingleton> leaf = null;

            if (type.IsClass)
            {
                leaf = provider.ClassSingletons.GetOrCreateLeaf(typeAsKeyArray);
            }
            else
            {
                leaf = provider.InterfaceSingletons.GetOrCreateLeaf(typeAsKeyArray);
            }

            foreach (IBProviderSingleton list in leaf.TraverseAllSubElements())
            {
                providers.Add(list);

                list.OnChanged += HandleChanged;
            }
        }

        private void HandleChanged( object oldValue , object newValue)
        {
            OnChanged?.Invoke((T)oldValue , (T) newValue);
        }

        void IDisposable.Dispose()
        {
            foreach (IBProviderSingleton provider in providers)
            {
                provider.OnChanged -= HandleChanged;
            }

            providers.Clear();
        }
    }
}