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
    // A Test behaves as an ordinary method
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

    // A Test behaves as an ordinary method
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


    }
}
