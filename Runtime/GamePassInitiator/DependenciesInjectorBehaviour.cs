using Bloodthirst.Core.Setup;
using Bloodthirst.Scripts.Core.GamePassInitiator;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.Utils;
using Bloodthirst.Core.SceneManager.DependencyInjector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Bloodthirst.Core.GameInitPass
{
    public class DependenciesInjectorBehaviour : MonoBehaviour, IPreGameSetup, IPostGameSetup
    {
        [SerializeField]
        private int preGameSetupOrder;

        
#if ODIN_INSPECTOR
[ReadOnly]
#endif

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

            List<IScriptableObject> scInstances = allScriptables.OfType<IScriptableObject>().ToList();
            List<IScriptableObjectSingleton> scSingletons = allScriptables.OfType<IScriptableObjectSingleton>().ToList();

            // instances
            foreach (IScriptableObject s in scInstances)
            {
                Type t = s.GetType();

                Type[] interfaces = t.GetInterfaces();
                Type genericType = interfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IScriptableObject<>));

                if (genericType == null)
                    continue;

                if (!genericType.IsGenericType)
                {
                    scProvider.RegisterInstance(t, s);
                }
                else
                {
                    Type genericParam = genericType.GetGenericArguments()[0];
                    scProvider.RegisterInstance(genericParam, s);
                }
            }

            // singletons
            foreach (IScriptableObjectSingleton s in scSingletons)
            {
                Type t = s.GetType();

                Type[] interfaces = t.GetInterfaces();
                Type genericType = interfaces.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IScriptableObjectSingleton<>));

                if (!genericType.IsGenericType)
                {
                    scProvider.RegisterInstance(t, s);
                }
                else
                {
                    Type genericParam = genericType.GetGenericArguments()[0];
                    scProvider.RegisterInstance(genericParam, s);
                }
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
