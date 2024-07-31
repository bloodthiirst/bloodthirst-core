using System;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.AssetProcessing
{
    public static class AssetWatcher
    {
        public static event Action<AssetImporter> OnAssetCreated;
        public static event Action<AssetImporter> OnAssetEdited;
        public static event Action<AssetImporter> OnAssetRemoved;
        public static event Action<AssetImporter> OnAssetMoved;

        internal static void TriggerAssetCreated(AssetImporter assetImporter)
        {
            OnAssetCreated?.Invoke(assetImporter);
        }

        internal static void TriggerAssetEdited(AssetImporter assetImporter)
        {
            OnAssetEdited?.Invoke(assetImporter);
        }

        internal static void TriggerAssetRemoved(AssetImporter assetImporter)
        {
            OnAssetRemoved?.Invoke(assetImporter);
        }

        internal static void TriggerAssetMoved(AssetImporter assetImporter)
        {
            OnAssetMoved?.Invoke(assetImporter);
        }
    }


    public class AssetPostProcessor : AssetPostprocessor
    {
        void OnPreprocessAsset()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_ASSET_WATCHER)
                return;

            bool hasImportFile = !assetImporter.importSettingsMissing;

            if (hasImportFile)
            {
                AssetWatcher.TriggerAssetEdited(assetImporter);
            }
            else
            {
                AssetWatcher.TriggerAssetCreated(assetImporter);
            }
        }
    }

    public class ScriptAssetModificationProcessor : AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions removeAssetOptions)
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_ASSET_WATCHER)
                return AssetDeleteResult.DidNotDelete;

            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            AssetWatcher.TriggerAssetRemoved(assetImporter);

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
