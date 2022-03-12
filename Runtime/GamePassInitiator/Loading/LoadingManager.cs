using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Core.Setup;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Bloodthirst.Core.Setup.GameStart;

namespace Bloodthirst.Core.SceneManager
{
    public enum LOADDING_STATE
    {
        FREE,
        LOADING
    }
    public class LoadingManager : MonoBehaviour , IPreGameSetup
#if UNITY_EDITOR
        , IPreprocessBuildWithReport
#endif
    {
#if UNITY_EDITOR
        public int callbackOrder => 100;

        public void OnPreprocessBuild(BuildReport report)
        {
            ScenesListData.Instance.LoadAllScenesAvailable();
        }
#endif
        public event Action<LoadingManager> OnLoadingValueChanged;

        public event Action<LoadingManager> OnLoadingStatusChanged;

        [SerializeField]
        private float progress;

        [SerializeField]
        private LOADDING_STATE state;

        public float Progress
        {
            get => progress;
            private set
            {
                if (progress == value)
                    return;

                progress = value;
                OnLoadingValueChanged?.Invoke(this);
            }
        }


        public LOADDING_STATE State
        {
            get => state;
            private set
            {
                if (state == value)
                    return;

                state = value;
                OnLoadingStatusChanged?.Invoke(this);
            }
        }

        void IPreGameSetup.Execute()
        {
            BProviderRuntime.Instance.RegisterSingleton(this);
        }

        public void Load(IEnumerable<IAsynOperationWrapper> ops)
        {
            StartCoroutine(CrtLoad(ops));
        }

        private IEnumerator CrtLoad(IEnumerable<IAsynOperationWrapper> ops)
        {
            progress = 0;
            state = LOADDING_STATE.LOADING;

            float totalOpsCount = ops.Sum(o => o.OperationsCount());

            List<IAsynOperationWrapper> ordered = ops.OrderBy(o => o.Order).ToList();

            float opsDone = 0;

            while (opsDone != totalOpsCount)
            {
                for (int i = 0; i < ordered.Count; i++)
                {
                    IAsynOperationWrapper currWave = ordered[i];

                    IEnumerable<AsyncOperation> startOps = currWave.StartOperations();

                    // get the wave of async ops
                    AsyncOperationGroup group = new AsyncOperationGroup(startOps);

                    do
                    {
                        group.Refresh();
                        float groupProgress = group.Progress * group.Count;
                        progress = (opsDone + groupProgress) / totalOpsCount;
                        yield return null;
                    }
                    while (!group.IsDone);

                    opsDone += group.Count;
                }
            }

            progress = 1f;
            state = LOADDING_STATE.FREE;

            progress = 0f;
        }


    }

}