using Bloodthirst.Core.PersistantAsset;
using System.IO;
using UnityEditor;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.Editor.EditorOpenTracker
{
    public class EditorOpenTracker
    {
        private const string TRACKER_DATA = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/EditorOpenTracker/TrackerData";

        public static bool IsFirstTime()
        {
            string absolute = Path.GetFullPath(TRACKER_DATA);

            if (!File.Exists(absolute))
            {
                File.Create(absolute);
                return true;
            }

            return false;
        }

        [UnityEditor.Callbacks.DidReloadScripts(SingletonScriptableObjectInit.EDITOR_OPEN_TRACKER)]
        public static void ReloadUpdater()
        {
            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }

        private static void OnEditorQuiting()
        {
            string absolute = Path.GetFullPath(TRACKER_DATA);

            File.Delete(absolute);
        }
    }
}
