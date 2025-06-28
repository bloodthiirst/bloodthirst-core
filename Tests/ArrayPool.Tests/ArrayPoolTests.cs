using System.Collections.Generic;
using System.Linq;
using Bloodthirst.Core;
using NUnit.Framework;
using UnityEngine;

public class ArrayPoolTests
{
    [Test]
    public void Test_Using_Disposable_Pattern()
    {
        ArrayPool<int> inst = ArrayPool<int>.Instance;

        const int size = 5;
        using (ArrayPool<int>.Instance.Get(size, out PooledArray<int> tmp))
        {
            Assert.IsTrue(tmp.length == size);
            Assert.IsTrue(tmp.array.Length >= size);
        }
    }

    [Test]
    public void Test_Using_Enumerator_Pattern()
    {
        ArrayPool<int> inst = ArrayPool<int>.Instance;

        const int size = 5;
        using (ArrayPool<int>.Instance.Get(size, out PooledArray<int> tmp))
        {
            for (int i = 0; i < size; ++i)
            {
                tmp[i] = i;
            }

            // count
            {
                IEnumerable<int> enumerable = tmp;
                int enumCounter = enumerable.Count();
                Assert.IsTrue(enumCounter == tmp.length);
            }

            // compare vals
            {
                int counter = 0;
                IEnumerable<int> enumerable = tmp;
                foreach (int curr in enumerable)
                {
                    Debug.Log($"Counter {counter} , Value {curr}");

                    Assert.IsTrue(curr == tmp[counter]);
                    counter++;
                }
            }

        }
    }
}
