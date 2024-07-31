using System;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public abstract class CameraControllerBase<T> : MonoBehaviour, ICameraController where T : CameraControllerBase<T>
    {
        public bool isEnabled { get; set; }

        public abstract void ApplyTransform(out Vector3 position, out Quaternion rotation);

        protected CameraManager _cameraManager;

        public void Register(CameraManager cameraManager)
        {
            _cameraManager = cameraManager;
            _cameraManager.RegisterCamera(this);
            OnRegister(cameraManager);
        }
        public void Unregister(CameraManager cameraManager)
        {
            _cameraManager.RemoveCamera(this);
            OnUnregister(cameraManager);
            _cameraManager = null;
        }

        public virtual void OnRegister(CameraManager cameraManager) { }
        public virtual void OnUnregister(CameraManager cameraManager) { }
        public abstract void OnCameraControllerSelected(bool isImmidiate);


    }
}
