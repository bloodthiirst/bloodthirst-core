﻿using System;

namespace Bloodthirst.Core.BProvider
{
    public class BSingleton<T> where T : class
    {
        public event Action<T> OnSingletonChanged;

        private T val;
        internal T Value
        {
            get => val;
            set
            {
                if (val != value)
                {
                    val = value;
                    OnSingletonChanged?.Invoke(val);
                }
            }
        }

        internal object TyplessValue
        {
            set
            {
                if (val != value)
                {
                    val = (T)value;
                    OnSingletonChanged?.Invoke(val);
                }
            }
        }

        public T Get => Value;

        internal BSingleton(T val)
        {
            this.val = val;
        }

        public static implicit operator T(BSingleton<T> val)
        {
            return val.Value;
        }

    }
}
