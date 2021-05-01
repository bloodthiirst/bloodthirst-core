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

        [KeepOriginalReference]
        public BClassWithKeepReference KeepReference { get; set; }
    }

    private BCopier<BClassWithKeepReference> instanceKeepRefCopier;

    private BCopierSettings settings;

    public OverrideTests()
    {
        instanceKeepRefCopier = BDeepCopy.GetCopier<BClassWithKeepReference>();

        settings = new BCopierSettings();
        settings.Add(new KeepOriginalReferenceOverride());
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
}
