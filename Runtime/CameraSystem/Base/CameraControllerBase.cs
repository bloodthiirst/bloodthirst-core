using Assets.Scripts.Core.GamePassInitiator;
using Bloodthirst.Core.UnitySingleton;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem {
    public abstract class CameraControllerBase<T> : UnitySingleton<T> , ICameraController, IAwakePass where T : CameraControllerBase<T> {
        public bool isEnabled { get; set; }

        public abstract void ApplyTransform(out Vector3 position, out Quaternion rotation);


        private void OnEnable() {
            CameraManager.RemoveCamera(this);
            CameraManager.RegisterCamera(this);
        }

        private void OnDisable() {
            CameraManager.RemoveCamera(this);
        }

        public void DoAwakePass()
        {
            CameraManager.RegisterCamera(this);
        }
    }
}
