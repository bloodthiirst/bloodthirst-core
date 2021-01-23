using Bloodthirst.Core.UnitySingleton;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Systems.CameraSystem
{
    public class CameraManager : UnitySingleton<CameraManager>, IEnablePass
    {

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

        public static void RegisterCamera(ICameraController camera)
        {
            Instance.AllCameras.Add(camera);
        }

        public static void RemoveCamera(ICameraController camera)
        {
            Instance?.AllCameras.Remove(camera);
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

            if (!isManagerActive)
                return;

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

            isInTransition = true;

            // get new position and rotation
            Vector3 position = default;
            Quaternion rotation = default;

            camera.ApplyTransform(out position, out rotation);


            // switch to the new camera
            bool doneRotation = false;
            bool donePosition = false;

            Sequence seqPosition = DOTween.Sequence()
                                    .Append(sceneCamera.transform.DOMove(position, transitionDuration))
                                    .SetEase(easeMode)
                                    .AppendCallback(() => donePosition = true)
                                    .OnComplete(() =>
                                    {
                                        if (donePosition && doneRotation)
                                        {
                                            camera.isEnabled = true;
                                            isInTransition = false;
                                            ActiveCamera = camera;
                                        }
                                    });



            Sequence seqRotation = DOTween.Sequence()
                                    .Append(sceneCamera.transform.DORotateQuaternion(rotation, transitionDuration))
                                    .SetEase(easeMode)
                                    .AppendCallback(() => doneRotation = true)
                                    .OnComplete(() =>
                                    {
                                        if (donePosition && doneRotation)
                                        {
                                            camera.isEnabled = true;
                                            isInTransition = false;
                                            ActiveCamera = camera;
                                        }
                                    });

            // play the transition
            seqPosition.Play();
            seqRotation.Play();
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
            Vector3 position = default;
            Quaternion rotation = default;

            camera.ApplyTransform(out position, out rotation);

            sceneCamera.transform.position = position;
            sceneCamera.transform.rotation = rotation;

            ActiveCamera = camera;

        }

        private void Update()
        {
            if (pause)
                return;
            if (!isManagerActive)
                return;
            if (isInTransition)
                return;
            if (ActiveCamera == null)
                return;
            if (!ActiveCamera.isEnabled)
                return;

            Vector3 pos = default;
            Quaternion rot = default;

            ActiveCamera.ApplyTransform(out pos, out rot);

            sceneCamera.transform.position = pos;
            sceneCamera.transform.rotation = rot;
        }

        public void DoEnablePass()
        {
            DisableAllCameras();

            isManagerActive = true;

            PickInitialCamera();


        }
    }
}
