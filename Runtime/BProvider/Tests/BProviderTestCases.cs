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
    public void TestSimpleAddAndGetWithTwoInstances()
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
    public void TestSimpleClassSingleton()
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
    public void TestSimpleInterfaceSingleton()
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
