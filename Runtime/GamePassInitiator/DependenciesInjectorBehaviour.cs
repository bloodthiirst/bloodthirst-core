using System;
using System.Linq;
using System.Collections.Generic;
using Bloodthirst.Core.Setup;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Utils;
using Bloodthirst.Core.SceneManager.DependencyInjector;
using Bloodthirst.Core.PersistantAsset;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Bloodthirst.Core.GameInitPass
{
    public class DependenciesInjectorBehaviour : MonoBehaviour, IPreGameSetup, IPostGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;

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

        int IPreGameSetup.Order => preGameSetupOrder;

        void IPreGameSetup.Execute()
        {
            ScriptableObjects();
        }

        void IPostGameSetup.Execute()
        {
            SceneDependencies();
        }

        private void ScriptableObjects()
        {
            BProvider.BProvider scProvider = new BProvider.BProvider();

            foreach(ScriptableObject sc in allScriptables)
            {
                Type scType = sc.GetType();

                if( sc is  ISingletonScriptableObject singleton)
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
            List<GameObject> allGOs = GameObjectUtils.GetAllRootGameObjects();

            // scene dependency for single scene stuff
            List<ISceneDependencyInjector> sceneDependencyInjector = GameObjectUtils.GetAllComponents<ISceneDependencyInjector>(allGOs, false);

            foreach (ISceneDependencyInjector inj in sceneDependencyInjector)
            {
                BProvider.BProvider injectionProvider = inj.GetProvider();
                BProviderRuntime.Instance.MergeWith(injectionProvider);
            }
        }

    }
}
