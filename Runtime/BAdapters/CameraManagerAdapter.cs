using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Systems.CameraSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(CameraManager))]
    [RequireComponent(typeof(CameraManager))]
    public class CameraManagerAdapter : MonoBehaviour , IPreGameSetup
    {
        public int Order => 0;

        void IPreGameSetup.Execute()
        {
            CameraManager cameraManager = GetComponent<CameraManager>();

            Assert.IsNotNull(cameraManager);

            BProviderRuntime.Instance.RegisterSingleton<CameraManager, CameraManager>(cameraManager);

            cameraManager.Initialize();
        }
    }
}
