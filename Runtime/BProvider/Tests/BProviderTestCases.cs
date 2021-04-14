using System.Collections.Generic;
using System.Linq;
using Bloodthirst.Core.ServiceProvider;
using NUnit.Framework;

public class BProviderTestCases
{
  
    private abstract class BaseClass{ }
    private class Child1Class : BaseClass , IChildClass { }
    private class Child2Class : BaseClass , IChildClass { }
    private interface IChildClass  { }

    private Child1Class child1Class;
    private Child2Class child2Class;

    public BProviderTestCases()
    {
        child1Class = new Child1Class();
        child2Class = new Child2Class();
    }

    [Test]
    public void TestSimpleAddAndGetInstance()
    {
        BProvider provider = new BProvider();

        provider.RegisterInstance(child1Class);

        IEnumerable<Child1Class> DIQuery = provider.GetInstances<Child1Class>();

        Assert.IsTrue(DIQuery.Count() == 1);

        Assert.AreEqual(child1Class, DIQuery.First());

    }

    [Test]
    public void MergeProvidersInterfaceInstances()
    {
        BProvider provider = new BProvider();

        provider.RegisterInstance<Child1Class , IChildClass>(child1Class);
        provider.RegisterInstance<Child2Class , IChildClass>(child2Class);

        BProvider copy = new BProvider().MergeWith(provider);

        IEnumerable<IChildClass> DIQuery1_original = provider.GetInstances<Child1Class>();
        IEnumerable<IChildClass> DIQuery1_copy = copy.GetInstances<Child1Class>();

        CollectionAssert.AreEquivalent(DIQuery1_copy, DIQuery1_original);

    }
    [Test]
    public void MergeProvidersClassInstances()
    {
        BProvider provider = new BProvider();

        provider.RegisterInstance(child1Class);
        provider.RegisterInstance(child2Class);

        BProvider copy = new BProvider().MergeWith(provider);

        IEnumerable<Child1Class> DIQuery1_original = provider.GetInstances<Child1Class>();
        IEnumerable<Child2Class> DIQuery2_original = provider.GetInstances<Child2Class>();

        IEnumerable<Child1Class> DIQuery1_copy = copy.GetInstances<Child1Class>();
        IEnumerable<Child2Class> DIQuery2_copy = copy.GetInstances<Child2Class>();

        CollectionAssert.AreEquivalent(DIQuery1_copy, DIQuery1_original);
        CollectionAssert.AreEquivalent(DIQuery2_copy, DIQuery2_original);
    }

    [Test]
    public void MergeProvidersInterfaceSingleton()
    {
        BProvider provider = new BProvider();

        provider.RegisterSingleton<Child1Class , IChildClass>(child1Class);

        BProvider copy = new BProvider().MergeWith(provider);

        IChildClass DIQuery1_original = provider.GetSingleton<IChildClass>().Get;
        IChildClass DIQuery1_copy = copy.GetSingleton<IChildClass>().Get;

        Assert.AreEqual(DIQuery1_copy, DIQuery1_original);
    }

    [Test]
    public void MergeProvidersClassSingleton()
    {
        BProvider provider = new BProvider();

        provider.RegisterSingleton(child1Class);

        BProvider copy = new BProvider().MergeWith(provider);

        Child1Class DIQuery1_original = provider.GetSingleton<Child1Class>().Get;
        Child1Class DIQuery1_copy = copy.GetSingleton<Child1Class>().Get;

        Assert.AreEqual(DIQuery1_copy, DIQuery1_original);
    }


    [Test]
    public void SimpleAddAndGetWithTwoInstances()
    {
        BProvider provider = new BProvider();

        provider.RegisterInstance(child1Class);
        provider.RegisterInstance(child2Class);

        IEnumerable<Child1Class> DIQuery1 = provider.GetInstances<Child1Class>();
        IEnumerable<Child2Class> DIQuery2 = provider.GetInstances<Child2Class>();

        Assert.IsTrue(DIQuery1.Count() == 1);
        Assert.IsTrue(DIQuery2.Count() == 1);

        Assert.AreEqual(child1Class, DIQuery1.First());
        Assert.AreEqual(child2Class, DIQuery2.First());

        List<BaseClass> monoBehaviours = provider.GetInstances<BaseClass>().ToList();
        Assert.IsTrue(monoBehaviours.Contains(child1Class));
        Assert.IsTrue(monoBehaviours.Contains(child2Class));

    }

    [Test]
    public void SimpleClassSingleton()
    {
        BProvider provider = new BProvider();

        bool firstSingletonRes = provider.RegisterSingleton(child1Class);

        BSingleton<Child1Class> DIQuery1 = provider.GetSingleton<Child1Class>();

        bool sameSingletonRes = provider.RegisterSingleton(child1Class);

        Child1Class anotherSingleton = new Child1Class();

        bool secondSingeton = provider.RegisterSingleton(anotherSingleton);

        Assert.AreEqual(child1Class, DIQuery1.Get);

        Assert.IsTrue(firstSingletonRes);

        Assert.IsTrue(sameSingletonRes);

        Assert.IsFalse(secondSingeton);
    }

    [Test]
    public void SimpleInterfaceSingleton()
    {
        BProvider provider = new BProvider();

        bool firstSingletonRes = provider.RegisterSingleton<Child1Class,IChildClass>(child1Class);

        BSingleton<IChildClass> DIQuery1 = provider.GetSingleton<IChildClass>();

        bool sameSingletonRes = provider.RegisterSingleton(child1Class);

        Child1Class anotherSingleton = new Child1Class();

        bool secondSingeton = provider.RegisterSingleton<Child1Class, IChildClass>(anotherSingleton);

        Assert.AreEqual(child1Class, DIQuery1.Get);
        Assert.IsTrue(firstSingletonRes);
        Assert.IsTrue(sameSingletonRes);
        Assert.IsFalse(secondSingeton);
    }
}
