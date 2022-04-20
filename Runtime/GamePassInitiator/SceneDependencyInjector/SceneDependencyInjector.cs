using Bloodthirst.Core.BProvider;

namespace Bloodthirst.Core.SceneManager.DependencyInjector
{
    public interface ISceneDependencyInjector
    {
        BProvider.BProvider GetProvider();
    }
}
