using Bloodthirst.Core.PersistantAsset;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.com.bloodthirst.bloodthirst_core.Runtime.Editor.EditorOpenTracker
{
    [InitializeOnLoad]
    public class EditorOpenTracker
    {
        private const string TRACKER_DATA = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/EditorOpenTracker/IgnoreAssets~/TrackerData";

        static EditorOpenTracker()
        {
            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }

        public static bool IsFirstTime()
        {
            string absolute = Path.GetFullPath(TRACKER_DATA);

            if (!File.Exists(absolute))
            {
                File.Create(TRACKER_DATA);
                return true;
            }

            return false;
        }

        private static void OnEditorQuiting()
        {
            string absolute = Path.GetFullPath(TRACKER_DATA);

            EditorApplication.quitting -= OnEditorQuiting;
            File.Delete(absolute);
        }
    }
}
