using Bloodthirst.Editor;
using UnityEditor;

namespace Bloodthirst.Utils.EditorOpenTracker
{
    [InitializeOnLoad]
    public class EditorStartupTracker
    {
        private const string IS_FIRST_LAUNCH_KEY = "_IsFirstLaunch";

        static EditorStartupTracker()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_STARTUP_TRACKER)
                return;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }

        public static bool IsFirstTime()
        {
            int isFirst = SessionState.GetInt(IS_FIRST_LAUNCH_KEY, -1);

            if (isFirst == -1)
            {
                SessionState.SetInt(IS_FIRST_LAUNCH_KEY, 1);
                return true;
            }

            return false;
        }

        private static void OnEditorQuiting()
        {
            EditorApplication.quitting -= OnEditorQuiting;
            SessionState.EraseInt(IS_FIRST_LAUNCH_KEY);
        }
    }
}
