using Bloodthirst.Core.Singleton;
using Bloodthirst.Scripts.Utils;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public class CameraManager : BSingleton<CameraManager>
    {
        public struct CameraState
        {
            public Vector3 position;
            public Quaternion rotation;
        }

        [ShowInInspector]
        HashSet<ICameraController> AllCameras = new HashSet<ICameraController>();

        [ShowInInspector]
        public ICameraController ActiveCamera { get; private set; }

        [SerializeField]
        private Camera sceneCamera = default;
        public Camera SceneCamera => sceneCamera;

        [SerializeField]
        private float transitionDuration = default;

        [SerializeField]
        private ICameraController initCamera = default;

        [SerializeField]
        public Ease easeMode;

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

        private Action<CameraState, CameraState>[] onChangedLookup = new Action<CameraState, CameraState>[2];

        private void HandleOnChangedNoOp(CameraState prev , CameraState curr)
        {

        }

        private void HandleOnChangedCallback(CameraState prev, CameraState curr)
        {
            OnCameraViewChanged?.Invoke(prev, curr);
        }

        public override void OnSetupSingleton()
        {
            previousPosition = sceneCamera.transform.position;
            previousRotation = sceneCamera.transform.rotation;

            onChangedLookup[0] = HandleOnChangedNoOp;
            onChangedLookup[1] = HandleOnChangedCallback;


            DisableAllCameras();

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

        private void DisableAllCameras()
        {
            foreach (ICameraController camera in AllCameras)
            {
                camera.isEnabled = false;
            }
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
                ActiveCamera.isEnabled = false;
                ActiveCamera = null;
            }

            ActiveCamera = camera;
            ActiveCamera.isEnabled = false;

            isInTransition = true;

            // get new position and rotation
            Vector3 position = default;
            Quaternion rotation = default;

            camera.ApplyTransform(out position, out rotation);

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

            camera.OnCameraControllerSelected(false);
        }

        private void OnTransitionComplete()
        {
            ActiveCamera.isEnabled = true;
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
                ActiveCamera.isEnabled = false;
                ActiveCamera = null;
            }

            // get new position and rotation
            camera.ApplyTransform(out Vector3 position, out Quaternion rotation);

            sceneCamera.transform.position = position;
            sceneCamera.transform.rotation = rotation;

            ActiveCamera = camera;

            camera.OnCameraControllerSelected(true);

        }

        private void Update()
        {
            if (pause || !isManagerActive || isInTransition || ActiveCamera == null || !ActiveCamera.isEnabled)
                return;

            ActiveCamera.ApplyTransform(out Vector3 pos, out Quaternion rot);

            UpdateCameraView(pos, rot);
        }

        private void UpdateCameraView(Vector3 pos, Quaternion rot)
        {
            CameraState prevState = new CameraState() { position = previousPosition, rotation = previousRotation };
            CameraState currState = new CameraState() { position = pos, rotation = rot };

            bool hasChanged = previousRotation != rot || previousPosition != pos;

            int asInt = BitUtils.Reinterpret<bool, int>(hasChanged);

            previousPosition = pos;
            previousRotation = rot;

            sceneCamera.transform.SetPositionAndRotation(pos, rot);

            onChangedLookup[asInt](prevState, currState);
        }
    }
}
