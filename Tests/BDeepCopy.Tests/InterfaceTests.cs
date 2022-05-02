using System.Diagnostics;
using Bloodthirst.BDeepCopy;
using NUnit.Framework;

public class InterfaceTests
{
    private class BClassWithRecursion : IBClass
    {
        public int id;
        public int Age { get; set; }
        public string Name { get; set; }
        public BClassWithRecursion Recursion { get; set; }
    }

    private class BClass : IBClass
    {
        public int id;
        public int Age { get; set; }
        public string Name { get; set; }
    }

    private interface IBClass
    {
        public string Name { get; set; }
    }

    private BCopier<IBClass> interfaceCopier;

    private BCopierSettings settings;

    public InterfaceTests()
    {
        interfaceCopier = BDeepCopy.GetCopier<IBClass>();

        settings = new BCopierSettings();
    }

    [Test]
    public void SimpleInterfaceCopy()
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

        IBClass copy = interfaceCopier.Copy(original);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(IBClass)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.IsInstanceOf<BClass>(original);
        Assert.IsInstanceOf<BClass>(copy);
        Assert.AreSame(original.Name, copy.Name);
    }

    [Test]
    public void SimpleInterfaceCopyWithRecursion()
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

        IBClass copy = interfaceCopier.Copy(original);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(IBClass)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.IsInstanceOf<BClassWithRecursion>(original);
        Assert.IsInstanceOf<BClassWithRecursion>(copy);
        Assert.AreSame(original.Name, copy.Name);
    }


}
