using System;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.AssetProcessing
{
    public static class ScriptAssetWatcher
    {
        public static event Action<MonoImporter> OnScriptCreated;
        public static event Action<MonoImporter> OnScriptEdited;
        public static event Action<MonoImporter> OnScriptRemoved;
        public static event Action<MonoImporter> OnScriptMoved;

        internal static void  TriggerScriptCreated(MonoImporter monoImporter)
        {
            OnScriptCreated?.Invoke(monoImporter);
        }

        internal static void TriggerScriptEdited(MonoImporter monoImporter)
        {
            OnScriptEdited?.Invoke(monoImporter);
        }

        internal static void TriggerScriptRemoved(MonoImporter monoImporter)
        {
            OnScriptRemoved?.Invoke(monoImporter);
        }

        internal static void TriggerScriptMoved(MonoImporter monoImporter)
        {
            OnScriptMoved?.Invoke(monoImporter);
        }
    }


    public class ScriptAssetPostProcessor : AssetPostprocessor
    {
        void OnPreprocessAsset()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_SCRIPTWATCHER)
                return;

            if (!(assetImporter is MonoImporter monoImporter))
                return;

            bool hasImportFile = !assetImporter.importSettingsMissing;

            if (hasImportFile)
            {
                //Debug.Log($"Script imported : {monoImporter.assetPath}");
                ScriptAssetWatcher.TriggerScriptEdited(monoImporter);
            }
            else
            {
                //Debug.Log($"NEW Script imported : {monoImporter.assetPath}");
                ScriptAssetWatcher.TriggerScriptCreated(monoImporter);
            }
        }
    }

    public class ScriptAssetModificationProcessor : AssetModificationProcessor
    {

        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions removeAssetOptions)
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_SCRIPTWATCHER)
                return AssetDeleteResult.DidNotDelete;

            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);

            if(assetImporter is MonoImporter monoImporter)
            {
                ScriptAssetWatcher.TriggerScriptRemoved(monoImporter);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
