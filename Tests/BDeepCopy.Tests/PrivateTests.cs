using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bloodthirst.BDeepCopy;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PrivateMembersTests
{
    private class BClassPrivate
    {
        private int id;

        public int Id => id;
        
        public int Age { get; private set; }

        public string Name { get; private set; }

        public BClassPrivate()
        {

        }
        public BClassPrivate(int id, int age, string name)
        {
            this.id = id;
            Age = age;
            Name = name;
        }
    }

    private BCopier<BClassPrivate> instancePrivateCopier;

    private BCopierSettings settings;

    public PrivateMembersTests()
    {
        instancePrivateCopier = BDeepCopy.GetCopier<BClassPrivate>();
    }


    [Test]
    public void CopyWithPrivateMembers()
    {
        // ARRAGE
        BClassPrivate original = new BClassPrivate(13 ,23,"Bloodthirst");

        Stopwatch profile = new Stopwatch();

        // ACT
        profile.Start();

        BClassPrivate copy = instancePrivateCopier.Copy(original);

        profile.Stop();

        UnityEngine.Debug.Log($"Time for copying {nameof(BClassPrivate)} : { profile.ElapsedTicks } ticks = { profile.ElapsedMilliseconds } ms ");

        // ASSERT
        Assert.AreNotSame(original, copy);
        Assert.AreEqual(original.Id, copy.Id);
        Assert.AreEqual(original.Age, copy.Age);
        Assert.AreSame(original.Name, copy.Name);
    }
}
