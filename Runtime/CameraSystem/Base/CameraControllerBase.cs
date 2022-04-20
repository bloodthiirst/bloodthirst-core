using Bloodthirst.Core.Singleton;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public abstract class CameraControllerBase<T> :BSingleton<T>, ICameraController where T : CameraControllerBase<T>
    {
        public bool isEnabled { get; set; }

        public abstract void ApplyTransform(out Vector3 position, out Quaternion rotation);

        protected CameraManager _cameraManager;

        void ICameraController.Initialize(CameraManager cameraManager)
        {
            _cameraManager.RemoveCamera(this);
            _cameraManager.RegisterCamera(this);

            OnQuerySingletonPass();
        }

        protected virtual void OnQuerySingletonPass() { }

        private void OnDisable()
        {
            _cameraManager.RemoveCamera(this);
        }

        public abstract void OnCameraControllerSelected(bool isImmidiate);
    }
}
