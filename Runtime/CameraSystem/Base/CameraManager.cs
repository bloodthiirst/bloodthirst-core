using Bloodthirst.Scripts.Utils;

#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public class CameraManager  : MonoBehaviour
    {
        public struct CameraState
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        private delegate void OnCameraChangedEvent(CameraState oldState , CameraState newState);

        private static readonly Type callbackType = typeof(OnCameraChangedEvent);

        
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        HashSet<ICameraController> AllCameras = new HashSet<ICameraController>();

        
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        public ICameraController ActiveCamera { get; private set; }

        [SerializeField]
        private Camera sceneCamera = default;
        public Camera SceneCamera => sceneCamera;

        [SerializeField]
        private float transitionDuration = default;

        [SerializeField]
        private ICameraController initCamera = default;

        [SerializeField]
        public bool isInTransition;

        [SerializeField]
        private bool isManagerActive;

        [SerializeField]
        private bool pause;
        public bool Pause { get => pause; set => pause = value; }

        private Vector3 previousPosition;
        private Quaternion previousRotation;

        public event Action<CameraState, CameraState> OnCameraViewChanged;

        private OnCameraChangedEvent[] onChangedLookup = new OnCameraChangedEvent[2];

        private void HandleOnChangedNoOp(CameraState prev , CameraState curr)
        {

        }

        private void HandleOnChangedCallback(CameraState prev, CameraState curr)
        {
            OnCameraViewChanged?.Invoke(prev, curr);
        }

        public void Initialize()
        {
            previousPosition = sceneCamera.transform.position;
            previousRotation = sceneCamera.transform.rotation;

            onChangedLookup[0] = HandleOnChangedNoOp;
            onChangedLookup[1] = HandleOnChangedCallback;

            isManagerActive = true;

            PickInitialCamera();
        }

        public void RegisterCamera(ICameraController camera)
        {
            AllCameras.Add(camera);
        }

        public void RemoveCamera(ICameraController camera)
        {
            AllCameras.Remove(camera);
        }

        private void PickInitialCamera()
        {
            if (initCamera == null)
                return;

            if (AllCameras.Contains(initCamera))
            {
                ChangeCamera(initCamera);
            }
        }

        public void ChangeCamera(ICameraController camera)
        {

            if (!isManagerActive || isInTransition || ActiveCamera == camera || !AllCameras.Contains(camera))
                return;

            // disable old camera
            if (ActiveCamera != null)
            {
                ActiveCamera = null;
            }

            ActiveCamera = camera;

            isInTransition = true;

            // get new position and rotation
            Vector3 position = default;
            Quaternion rotation = default;

            camera.ApplyTransform(out position, out rotation);

            /*
            Sequence seqPosition = DOTween.Sequence()
                                    .Append(sceneCamera.transform.DOMove(position, transitionDuration))
                                    .SetEase(easeMode);



            Sequence seqRotation = DOTween.Sequence()
                                    .Append(sceneCamera.transform.DORotateQuaternion(rotation, transitionDuration))
                                    .SetEase(easeMode);

            Sequence mergedPosAndRot = DOTween.Sequence()
                .Join(seqRotation)
                .Join(seqPosition)
                .OnComplete(OnTransitionComplete)
                .OnUpdate(OnTransitionUpdate);

            // play the transition
            mergedPosAndRot.Play();
            */
            // todo : remporary fix , to avoid using dotween
            sceneCamera.transform.SetLocalPositionAndRotation(position , rotation);
            camera.OnCameraControllerSelected(false);
        }

        private void OnTransitionComplete()
        {
            isInTransition = false;
        }
        private void OnTransitionUpdate()
        {
            Vector3 pos = sceneCamera.transform.position;
            Quaternion rot = sceneCamera.transform.rotation;
            UpdateCameraView(pos, rot);
        }

        public void ChangeCameraImmidiately(ICameraController camera)
        {
            if (isInTransition)
                return;

            if (ActiveCamera == camera)
                return;

            if (!AllCameras.Contains(camera))
                return;

            // disable old camera
            if (ActiveCamera != null)
            {
                ActiveCamera = null;
            }

            // get new position and rotation
            camera.ApplyTransform(out Vector3 position, out Quaternion rotation);

            sceneCamera.transform.position = position;
            sceneCamera.transform.rotation = rotation;

            ActiveCamera = camera;

            camera.OnCameraControllerSelected(true);

        }

        private void LateUpdate()
        {
            if (pause || !isManagerActive || isInTransition || ActiveCamera == null)
                return;

            ActiveCamera.ApplyTransform(out Vector3 pos, out Quaternion rot);

            UpdateCameraView(pos, rot);
        }

        private void UpdateCameraView(Vector3 pos, Quaternion rot)
        {
            CameraState prevState = new CameraState() { position = previousPosition, rotation = previousRotation };
            CameraState currState = new CameraState() { position = pos, rotation = rot };

            bool hasChanged = previousRotation != rot || previousPosition != pos;
            
            previousPosition = pos;
            previousRotation = rot;

            sceneCamera.transform.SetPositionAndRotation(pos, rot);

            if (hasChanged)
            {
                HandleOnChangedCallback(prevState, currState);
            }
        }
    }
}
