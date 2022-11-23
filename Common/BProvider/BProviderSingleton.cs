using System;

namespace Bloodthirst.Core.BProvider
{
    public class BProviderSingleton<T> : IBProviderSingleton
    {
        private Action<object,object> onChanged;
        event Action<object,object> IBProviderSingleton.OnChanged
        {
            add
            {
                onChanged += value;
            }

            remove
            {
                onChanged -= value;
            }
        }
        public event Action<T,T> OnChanged;

        private T _value;
        object IBProviderSingleton.Value
        {
            get => _value;
            set
            {
                T old = _value;
                _value = (T) value;
                OnChanged?.Invoke(old ,_value);
                onChanged?.Invoke(old,_value);
            }
        }

        public BProviderSingleton(T value)
        {
            _value = value;
        }
    }
}