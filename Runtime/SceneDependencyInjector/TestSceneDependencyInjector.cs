using Bloodthirst.Core.SceneManager.DependencyInjector;
using Bloodthirst.Core.ServiceProvider;
using UnityEngine;

public class TestSceneDependencyInjector : MonoBehaviour , ISceneDependencyInjector
{
    [SerializeField]
    private float test;
    BProvider ISceneDependencyInjector.GetProvider()
    {
        BProvider fakeProvider = new BProvider();
        fakeProvider.RegisterInstance<ISceneDependencyInjector>(this);

        return fakeProvider;
    }
}
