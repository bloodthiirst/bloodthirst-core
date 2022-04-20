#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    public class SceneStartup : MonoBehaviour
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
    }

}