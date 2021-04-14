using Bloodthirst.Core.ServiceProvider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.SceneManager.DependencyInjector
{
    public interface ISceneDependencyInjector
    {
        BProvider GetProvider();
    }
}
