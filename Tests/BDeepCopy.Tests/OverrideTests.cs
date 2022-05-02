using System.Diagnostics;
using Bloodthirst.BDeepCopy;
using NUnit.Framework;

public class OverrideTests
{
    private class BClassWithKeepReference
    {
        public int id;
        public int Age { get; set; }
        public string Name { get; set; }

        [KeepReference]
        public BClassWithKeepReference KeepReference { get; set; }
    }

    private class BClassWithIgnoreMember
    {
        [IgnoreMember]
        public int id;

        public int Age { get; set; }

        [IgnoreMember]
        public string Name { get; set; }

        [IgnoreMember]
        public BClassWithIgnoreMember KeepReference { get; set; }
    }

    private BCopier<BClassWithKeepReference> instanceKeepRefCopier;
    private BCopier<BClassWithIgnoreMember> instanceIgnoreMemberCopier;

    private BCopierSettings settings;

    public OverrideTests()
    {
        instanceKeepRefCopier = BDeepCopy.GetCopier<BClassWithKeepReference>();
        instanceIgnoreMemberCopier = BDeepCopy.GetCopier<BClassWithIgnoreMember>();

        settings = new BCopierSettings();
    }

    [Test]
    public void KeepSameReference()
    {
        // ARRAGE
        BClassWithKeepReference original = new BClassWithKeepReference()
        {
            id = 69,
            Age = 23,
            Name = "Bloodthirst"
        };

        original.KeepReference = original;

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        BClassWithKeepReference copy = instanceKeepRefCopier.Copy(original, settings);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(BClassWithKeepReference)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.AreEqual(original.id, copy.id);
        Assert.AreEqual(original.Age, copy.Age);
        Assert.AreSame(original.Name, copy.Name);
        Assert.AreSame(original.KeepReference, copy.KeepReference);
    }

    [Test]
    public void IgnoreMember()
    {
        // ARRAGE
        BClassWithIgnoreMember original = new BClassWithIgnoreMember()
        {
            id = 69,
            Age = 23,
            Name = "Bloodthirst"
        };

        original.KeepReference = original;

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        BClassWithIgnoreMember copy = instanceIgnoreMemberCopier.Copy(original, settings);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(BClassWithIgnoreMember)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.AreEqual(default(int) , copy.id);
        Assert.AreEqual(copy.Age, original.Age);
        Assert.AreSame(default(string) , copy.Name);
        Assert.AreSame(copy.KeepReference, default(BClassWithIgnoreMember));
    }
}
