using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core
{
    public struct PooledArray<T> : IEnumerable<T>
    {
        public T[] array;
        public int length;

        public ref T this[int index]
        {
            get
            {
                Assert.IsTrue(index > -1 && index < length);
                return ref array[index];
            }
        }

        private struct PooledArrayEnumerator : IEnumerator<T>
        {
            public PooledArray<T> arr;
            public int index;
            public T Current => arr.array[index];
            object IEnumerator.Current => Current;
            public void Dispose() { }
            public bool MoveNext()
            {
                index++;
                return index < arr.length;
            }
            public void Reset()
            {
                index = -1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new PooledArrayEnumerator() { arr = this, index = -1 };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct PooledArrayWrapper<T> : IDisposable
    {
        public PooledArray<T> pooledArray;

        public void Dispose()
        {
            ArrayPool<T>.Instance.Return(pooledArray);
        }
    }

    public class ArrayPool<T>
    {
        private class IntEqualityComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            public int GetHashCode(int obj)
            {
                return obj;
            }
        }

        private static readonly uint ElementSize;

        static ArrayPool()
        {
            Type t = typeof(T);

            T inst = default;

            ElementSize = (uint)(t.IsValueType ? Marshal.SizeOf(inst) : IntPtr.Size);
        }

        private static ArrayPool<T> instance;
        public static ArrayPool<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ArrayPool<T>();
                    instance.sizeToArrayQueueLookup = new Dictionary<int, Queue<T[]>>(32, new IntEqualityComparer());

                    for (int i = 0; i < 32; ++i)
                    {
                        instance.sizeToArrayQueueLookup.Add(1 << i, new Queue<T[]>(expandCountBy));
                    }
                }

                return instance;
            }
        }

        private const int expandCountBy = 8;

        private Dictionary<int, Queue<T[]>> sizeToArrayQueueLookup;

        public PooledArrayWrapper<T> Get(int length, out PooledArray<T> pooledArray)
        {
            int idx = Mathf.NextPowerOfTwo(length);
            bool exists = sizeToArrayQueueLookup.TryGetValue(idx, out Queue<T[]> queue);
            Assert.IsTrue(true);

            T[] arr = null;

            if (queue.Count == 0)
            {
                for (int i = 0; i < expandCountBy - 1; ++i)
                {
                    T[] newArr = new T[idx];
                    queue.Enqueue(newArr);
                }

                arr = new T[idx];
            }
            else
            {
                arr = queue.Dequeue();
            }

            Assert.IsNotNull(arr);

            pooledArray = new PooledArray<T>() { array = new T[idx], length = length };

            return new PooledArrayWrapper<T>()
            {
                pooledArray = pooledArray
            };
        }

        public void Return(PooledArray<T> pooledArray)
        {
            for(int i = 0; i < pooledArray.length; ++i)
            {
                pooledArray[i] = default;
            }

            int idx = Mathf.NextPowerOfTwo(pooledArray.length);
            bool exists = sizeToArrayQueueLookup.TryGetValue(idx, out Queue<T[]> queue);
            Assert.IsTrue(true);

            queue.Enqueue(pooledArray.array);
        }
    }
}
