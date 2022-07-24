using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.AssetProcessing;
using Bloodthirst.Editor.Commands;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace Bloodthirst.Core.GameEventSystem
{
    [InitializeOnLoad]
    internal class GameEventScriptWatcher
    {
        static GameEventScriptWatcher()
        {
            ScriptAssetWatcher.OnScriptRemoved -= HandleScriptRemoved;
            ScriptAssetWatcher.OnScriptRemoved += HandleScriptRemoved;
        }

        private static void HandleScriptRemoved(MonoImporter monoImporter)
        {
            MonoScript script = monoImporter.GetScript();

            if (script == null)
                return;

            Type implementedClass = script.GetClass();

            if (implementedClass == null)
                return;

            if (!GameEventSystemUtils.IsGameEventClass(implementedClass, out Type enumType))
                return;

            List<GameEventSystemAsset> allEventAssets = EditorUtils.FindAssets<GameEventSystemAsset>();

            for (int i = 0; i < allEventAssets.Count; i++)
            {
                GameEventSystemAsset a = allEventAssets[i];

                if (a.enumName != enumType.Name)
                    continue;

                a.RemoveByClass(implementedClass.Name);
                CommandManagerEditor.RunInstant(new RegenerateEnumScriptCommand(a));
            }


        }
    }
}
