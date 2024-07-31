using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public interface ICameraController
    {
        bool isEnabled { get; set; }

        void ApplyTransform(out Vector3 position, out Quaternion rotation);

        void OnCameraControllerSelected(bool isImmidiate);

        void Register(CameraManager cameraManager);
        void Unregister(CameraManager cameraManager);

    }
}
