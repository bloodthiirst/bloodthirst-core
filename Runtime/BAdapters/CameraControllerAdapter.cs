using Bloodthirst.Core.BProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Systems.CameraSystem;
using UnityEngine;

namespace Bloodthirst.Runtime.BAdapter
{
    [BAdapterFor(typeof(ICameraController))]
    [RequireComponent(typeof(ICameraController))]
    public class CameraControllerAdapter : MonoBehaviour, IQuerySingletonPass
    {
        void IQuerySingletonPass.Execute()
        {
            ICameraController cam = GetComponent<ICameraController>();
            CameraManager _cameraManager = BProviderRuntime.Instance.GetSingleton<CameraManager>();
            
            cam.Initialize(_cameraManager);
        }
    }
}
