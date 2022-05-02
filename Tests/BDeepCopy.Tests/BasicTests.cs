using System.Diagnostics;
using Bloodthirst.BDeepCopy;
using NUnit.Framework;

public class BasicTests
{
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

    private BCopierSettings settings;

    public BasicTests()
    {
        instanceCopier = BDeepCopy.GetCopier<BClass>();
        instanceRecursionCopier = BDeepCopy.GetCopier<BClassWithRecursion>();

        settings = new BCopierSettings();
    }

    [Test]
    public void SimpleObjectCopy()
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

    [Test]
    public void CopyWithRecursion()
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

}
