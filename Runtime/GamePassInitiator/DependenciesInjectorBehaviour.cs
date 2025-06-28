﻿using System;
using System.Linq;
using System.Collections.Generic;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Utils;
using Bloodthirst.Core.SceneManager.DependencyInjector;
using Bloodthirst.Core.PersistantAsset;
using UnityEngine;
using UnityEngine.Pool;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Bloodthirst.Core.GameInitPass
{
    public class DependenciesInjectorBehaviour : MonoBehaviour , IPostGameSetup
    {
        [SerializeField]
        private List<ScriptableObject> allScriptables;

#if ODIN_INSPECTOR
        [FolderPath]
#endif
        [SerializeField]
        private string[] searchInFolders;

#if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Button]
#endif

        private void FetchAllScriptableObjects()
        {
            allScriptables = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}", searchInFolders)
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => AssetDatabase.LoadAssetAtPath<ScriptableObject>(p))
                .ToList();
        }
#endif

        void IPostGameSetup.Execute()
        {
            SceneDependencies();
        }

        public void RegisterUnityObjects()
        {
            BProvider.BProvider scProvider = new BProvider.BProvider();

            foreach (ScriptableObject sc in allScriptables)
            {
                Type scType = sc.GetType();

                if (sc is ISingletonScriptableObject singleton)
                {
                    scProvider.RegisterSingleton(scType, singleton);
                    continue;
                }

                scProvider.RegisterInstance(scType, sc);
            }

            BProviderRuntime.Instance.MergeWith(scProvider);
        }

        private void SceneDependencies()
        {
            using (ListPool<GameObject>.Get(out List<GameObject> allGOs))
            using (ListPool<ISceneDependencyInjector>.Get(out List<ISceneDependencyInjector> sceneDependencyInjector))
            {           
                GameObjectUtils.GetAllRootGameObjects(allGOs);

                // scene dependency for single scene stuff
                GameObjectUtils.GetAllComponents(ref sceneDependencyInjector, allGOs, false);

                foreach (ISceneDependencyInjector inj in sceneDependencyInjector)
                {
                    BProvider.BProvider injectionProvider = inj.GetProvider();
                    BProviderRuntime.Instance.MergeWith(injectionProvider);
                }
            }
        }

    }
}
