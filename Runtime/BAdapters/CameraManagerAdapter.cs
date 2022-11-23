using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Systems.CameraSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(CameraManager))]
    [RequireComponent(typeof(CameraManager))]
    public class CameraManagerAdapter : MonoBehaviour, ISetupSingletonPass
    {
        void ISetupSingletonPass.Execute()
        {
            CameraManager cameraManager = GetComponent<CameraManager>();

            Assert.IsNotNull(cameraManager);

            BProviderRuntime.Instance.RegisterSingleton<CameraManager, CameraManager>(cameraManager);

            cameraManager.Initialize();
        }
    }
}
