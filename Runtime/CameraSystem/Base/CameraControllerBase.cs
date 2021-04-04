using Bloodthirst.Core.Pooling;
using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Singleton;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public abstract class CameraControllerBase<T> : UnitySingleton<T>, ICameraController, IQuerySingletonPass  where T : CameraControllerBase<T>
    {
        public bool isEnabled { get; set; }

        public abstract void ApplyTransform(out Vector3 position, out Quaternion rotation);

        protected CameraManager _cameraManager;

        void IQuerySingletonPass.Execute()
        {
            _cameraManager = BProviderRuntime.Instance.GetSingleton<CameraManager>();
            _cameraManager.RegisterCamera(this);
        }

        private void OnEnable()
        {
            _cameraManager.RemoveCamera(this);
            _cameraManager.RegisterCamera(this);
        }

        private void OnDisable()
        {
            _cameraManager.RemoveCamera(this);
        }



    }
}
