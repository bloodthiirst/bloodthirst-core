using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bloodthirst.BDeepCopy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Tests
{
    [SetUpFixture]

    private class BClassWithRecursion
    {
        public int id;
        public int Age { get; set; }
        public string Name { get; set; }
        public BClassWithRecursion Recursion { get; set; }
    }

    private class BClass
    {
        public int id;
        public int Age { get; set; }
        public string Name { get; set; }
    }

    private BCopier<BClass> instanceCopier;
    private BCopier<BClassWithRecursion> instanceRecursionCopier;
    private BCopier<List<BClass>> listCopier;
    private BCopier<Dictionary<int, BClass>> dictCopier;

    [OneTimeSetUp]
    public void SetupCaching()
    {
        instanceCopier = BDeepCopy.GetCopier<BClass>();
        instanceRecursionCopier = BDeepCopy.GetCopier<BClassWithRecursion>();
        listCopier = BDeepCopy.GetCopier<List<BClass>>();
        dictCopier = BDeepCopy.GetCopier<Dictionary<int, BClass>>();
    }

    [Test]
    public void TestSimpleObjectCopy()
    {
        // ARRAGE
        BClass original = new BClass()
        {
            id = 69,
            Age = 23,
            Name = "Bloodthirst"
        };

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        BClass copy = instanceCopier.Copy(original);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(BClass)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.AreEqual(original.id, copy.id);
        Assert.AreEqual(original.Age, copy.Age);
        Assert.AreSame(original.Name, copy.Name);
    }


    // A Test behaves as an ordinary method
    [Test]
    public void TestObjectCopyWithRecursion()
    {
        // ARRAGE
        BClassWithRecursion original = new BClassWithRecursion()
        {
            id = 69,
            Age = 23,
            Name = "Bloodthirst"
        };

        original.Recursion = original;

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        BClassWithRecursion copy = instanceRecursionCopier.Copy(original);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(BClassWithRecursion)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.AreNotSame(original.Recursion, copy.Recursion);

        Assert.AreSame(original, original.Recursion);
        Assert.AreSame(copy, copy.Recursion);

        Assert.AreEqual(original.id, copy.id);
        Assert.AreEqual(original.Age, copy.Age);
        Assert.AreSame(original.Name, copy.Name);
    }


    // A Test behaves as an ordinary method
    [Test]
    public void TestSimpleListCopy()
    {
        // ARRAGE
        BClass c1 = new BClass()
        {
            id = 69,
            Age = 23,
            Name = "Bloodthirst"
        };

        BClass c2 = new BClass()
        {
            id = 420,
            Age = 33,
            Name = "Ketsueki"
        };

        List<BClass> lst = new List<BClass>()
        {
            c1,
            c2
        };

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        List<BClass> copy = listCopier.Copy(lst);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(List<BClass>)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(lst, copy);
        Assert.AreEqual(lst.Count, copy.Count);

        if (lst.Count != copy.Count)
            return;

        for (int i = 0; i < lst.Count; i++)
        {
            Assert.AreNotSame(lst[i], copy[i]);
        }

        CollectionAssert.AreNotEqual(lst, copy);
        CollectionAssert.IsNotSubsetOf(lst, copy);
    }

    [Test]
    public void TestSimpleDictionaryCopy()
    {
        // ARRAGE
        BClass c1 = new BClass()
        {
            id = 69,
            Age = 23,
            Name = "Bloodthirst"
        };

        BClass c2 = new BClass()
        {
            id = 420,
            Age = 33,
            Name = "Ketsueki"
        };

        Dictionary<int, BClass> dict = new Dictionary<int, BClass>()
        {
            { 13 , c1 },
            { 666 , c2 }
        };

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        Dictionary<int, BClass> copy = dictCopier.Copy(dict);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(Dictionary<int, BClass>)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(dict, copy);
        Assert.AreEqual(dict.Count, copy.Count);

        if (dict.Count != copy.Count)
            return;

        for (int i = 0; i < copy.Count; i++)
        {
            var k1 = dict.Keys.ElementAt(i);
            var k2 = copy.Keys.ElementAt(i);

            var v1 = dict.Values.ElementAt(i);
            var v2 = copy.Values.ElementAt(i);

            Assert.AreEqual(k1, k2);
            Assert.AreNotSame(v1, v2);
        }
    }
}
