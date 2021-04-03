using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.System.CommandSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BProviderTestCases
{
    [Test]
    public void SimpleAddAndGetInstance()
    {
        BProvider provider = new BProvider();

        SceneLoadingManager sceneLoadingManager = new GameObject().AddComponent<SceneLoadingManager>();

        provider.RegisterType<SceneLoadingManager>();

        provider.RegisterInstance(sceneLoadingManager);

        IEnumerable<SceneLoadingManager> sceneLoadingManagergQuery = provider.GetInstances<SceneLoadingManager>();

        Assert.IsTrue(sceneLoadingManagergQuery.Count() == 1);

        Assert.AreEqual(sceneLoadingManager, sceneLoadingManagergQuery.First());

    }

    [Test]
    public void SimpleAddAndGet2Instances()
    {
        BProvider provider = new BProvider();

        SceneLoadingManager sceneLoadingManager = new GameObject().AddComponent<SceneLoadingManager>();
        CommandManagerBehaviour commandManagerBehaviour = new GameObject().AddComponent<CommandManagerBehaviour>();

        provider.RegisterType<SceneLoadingManager>();
        provider.RegisterType<SceneLoadingManager>();

        provider.RegisterInstance(sceneLoadingManager);
        provider.RegisterInstance(commandManagerBehaviour);

        IEnumerable<SceneLoadingManager> sceneLoadingManagergQuery = provider.GetInstances<SceneLoadingManager>();
        IEnumerable<CommandManagerBehaviour> commandManagerBehaviourQuery = provider.GetInstances<CommandManagerBehaviour>();

        Assert.IsTrue(sceneLoadingManagergQuery.Count() == 1);
        Assert.IsTrue(commandManagerBehaviourQuery.Count() == 1);

        Assert.AreEqual(sceneLoadingManager, sceneLoadingManagergQuery.First());
        Assert.AreEqual(commandManagerBehaviour, commandManagerBehaviourQuery.First());

        List<MonoBehaviour> monoBehaviours = provider.GetInstances<MonoBehaviour>().ToList();
        Assert.IsTrue(monoBehaviours.Contains(sceneLoadingManager));
        Assert.IsTrue(monoBehaviours.Contains(commandManagerBehaviour));

    }

    [Test]
    public void SimpleSingleton()
    {
        BProvider provider = new BProvider();

        SceneLoadingManager sceneLoadingManager = new GameObject().AddComponent<SceneLoadingManager>();

        SceneLoadingManager sceneLoadingManager2nd = new GameObject().AddComponent<SceneLoadingManager>();

        provider.RegisterType<SceneLoadingManager>();

        bool firstSingletonRes = provider.RegisterSingleton(sceneLoadingManager);

        SceneLoadingManager sceneLoadingManagergQuery = provider.GetSingleton<SceneLoadingManager>();

        bool sameSingletonRes = provider.RegisterSingleton(sceneLoadingManager);

        bool secondSingeton = provider.RegisterSingleton(sceneLoadingManager2nd);

        Assert.AreEqual(sceneLoadingManager, sceneLoadingManagergQuery);

        Assert.IsTrue(firstSingletonRes);

        Assert.IsTrue(sameSingletonRes);

        Assert.IsFalse(secondSingeton);
    }
}
