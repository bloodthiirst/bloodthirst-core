using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(InputUtils))]
    [RequireComponent(typeof(InputUtils))]
    public class InputUtilsAdapter : MonoBehaviour, IOnSceneLoaded
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            InputUtils globalPool = GetComponent<InputUtils>();

            Assert.IsNotNull(globalPool);

            BProviderRuntime.Instance.RegisterSingleton<InputUtils, InputUtils>(globalPool);

            globalPool.Initialize();
        }
    }
}
