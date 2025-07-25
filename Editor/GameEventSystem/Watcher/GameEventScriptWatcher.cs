﻿using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.AssetProcessing;
using Bloodthirst.Editor.Commands;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Pool;
using UnityEngine;

namespace Bloodthirst.Core.GameEventSystem
{
    [InitializeOnLoad]
    internal class GameEventScriptWatcher
    {
        static GameEventScriptWatcher()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_GAME_EVENT_SCRIPT_WATCHER)
                return;

            AssetWatcher.OnAssetRemoved -= HandleScriptRemoved;
            AssetWatcher.OnAssetRemoved += HandleScriptRemoved;
        }

        private static void HandleScriptRemoved(AssetImporter importer)
        {
            if (!(importer is MonoImporter monoImporter))
                return;

            MonoScript script = monoImporter.GetScript();

            if (script == null)
                return;

            Type implementedClass = script.GetClass();

            if (implementedClass == null)
                return;

            if (!GameEventSystemUtils.IsGameEventClass(implementedClass, out Type enumType))
                return;


            using (ListPool<GameEventSystemAsset>.Get(out List<GameEventSystemAsset> allEventAssets))
            {
                allEventAssets.AddRange(EditorUtils.FindAssetsByType<GameEventSystemAsset>());

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
}
