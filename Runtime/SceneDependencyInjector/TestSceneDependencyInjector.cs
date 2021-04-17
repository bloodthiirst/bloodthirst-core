using Bloodthirst.Core.SceneManager.DependencyInjector;
using Bloodthirst.Core.ServiceProvider;
using UnityEngine;

public class TestSceneDependencyInjector : MonoBehaviour , ISceneDependencyInjector
{
    [SerializeField]
    private ScriptableObject scriptableObject;

    BProvider ISceneDependencyInjector.GetProvider()
    {
        BProvider fakeProvider = new BProvider();
        fakeProvider.RegisterInstance<ScriptableObject>(scriptableObject);

        return fakeProvider;
    }
}
