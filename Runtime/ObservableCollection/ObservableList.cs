using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.Collections
{
    public class ObservableList<T> : List<T>
    {
        public event Action<T> OnItemRemoved;

        public event Action<T> OnItemAdded;

        public event Action<ObservableList<T>> OnListChanged;

        public event Action<ObservableList<T>> OnListCleared;

        public new void Add(T item)
        {
            base.Add(item);

            OnItemAdded?.Invoke(item);

            OnListChanged?.Invoke(this);
        }


        public new void Remove(T item)
        {
            base.Remove(item);

            OnItemRemoved?.Invoke(item);

            OnListChanged?.Invoke(this);
        }

        public new void Clear()
        {
            base.Clear();

            OnListCleared?.Invoke(this);

            OnListChanged?.Invoke(this);
        }
    }
}