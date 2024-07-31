using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Systems.CameraSystem;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(ICameraController))]
    [RequireComponent(typeof(ICameraController))]
    public class CameraControllerAdapter : MonoBehaviour, IOnSceneLoaded
    {
        void IOnSceneLoaded.OnLoaded(ISceneInstanceManager sceneInstance)
        {
            ICameraController cam = GetComponent<ICameraController>();
            CameraManager _cameraManager = BProviderRuntime.Instance.GetSingleton<CameraManager>();
            
            cam.Register(_cameraManager);
        }
    }
}
