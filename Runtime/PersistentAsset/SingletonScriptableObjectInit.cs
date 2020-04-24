﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using UnityEngine;

namespace Bloodthirst.Core.PersistantAsset
{
    public static class SingletonScriptableObjectInit
    {

#if UNITY_EDITOR

        #region DidReloadScripts conts

        public const int TRACK_ASSEMBLY_RELOAD = 0;

        public const int SINGLETONS_CREATION_CHECK = 1;

        public const int SINGLETONS_RELOAD = 2;

        public const int SCENE_CREATOR = 3;

        #endregion

        [DidReloadScripts(SINGLETONS_CREATION_CHECK)]
        public static void OnDidReloadScripts()
        {

            // get singleton types

            List<Type> validTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => !t.IsGenericType)
                .Where(t => t.GetInterfaces().Contains(typeof(ISingletonScriptableObject)))
                .ToList();



            // check

            foreach (Type type in validTypes)
            {
                PropertyInfo pathProp = type.GetProperty("AssetPath", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                string typePath = (string)pathProp.GetValue(null);

                string relativeFolderPath = typePath.Remove(typePath.Length - type.Name.Length - 1);

                string assetPath = "Assets/Resources/" + typePath + ".asset";

                // all instances available with the same type

                UnityEngine.Object[] allResources = Resources.LoadAll(string.Empty, type);

                ScriptableObject so = null;

                // if only one asset is available

                if (allResources.Length == 1)
                {

                    so = (ScriptableObject)allResources[0];

                    string currentAssetPath = AssetDatabase.GetAssetPath(so);

                    // if the asset isn't placed in the correct path

                    if (!currentAssetPath.Equals(assetPath))
                    {
                        AssetDatabase.CopyAsset(currentAssetPath, assetPath);
                        AssetDatabase.DeleteAsset(currentAssetPath);
                    }

                    continue;
                }

                // if more than one asset available

                if (allResources.Length > 1)
                {
                    // -1 means thesset doesnt exist

                    int index = -1;

                    // search for a correctly placed asset

                    for (int i = 0; i < allResources.Length; i++)
                    {
                        string currentAssetPath = AssetDatabase.GetAssetPath(allResources[i]);

                        if (currentAssetPath.Equals(assetPath))
                        {
                            index = i;
                            break;
                        }
                    }

                    // delete extra assets

                    for (int i = 0; i < allResources.Length; i++)
                    {
                        if (i == index)
                            continue;

                        string currentAssetPath = AssetDatabase.GetAssetPath(allResources[i]);

                        AssetDatabase.DeleteAsset(currentAssetPath);
                    }

                    // create an asset if it doesnt exist

                    if (index == -1)
                    {
                        so = ScriptableObject.CreateInstance(type);

                        // get the asset path

                        string folderPath = "Assets/Resources/" + relativeFolderPath;

                        CreateFoldersFromPath(folderPath);

                        AssetDatabase.CreateAsset(so, assetPath);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }

                    continue;
                }

                // if no asset is available

                if (allResources.Length == 0)
                {
                    so = ScriptableObject.CreateInstance(type);

                    // get the asset path

                    string folderPath = "Assets/Resources/" + relativeFolderPath;

                    CreateFoldersFromPath(folderPath);

                    AssetDatabase.CreateAsset(so, assetPath);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    continue;
                }

            }

            EditorApplication.playModeStateChanged -= EditorApplicationPlayModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplicationPlayModeStateChanged;

            Debug.Log("[ Persistant Assets successfully initialized ]");

        }

        static void CreateFoldersFromPath(string folderPath)
        {
            string[] folders = folderPath.Split('/');

            for (int i = 0; i < folders.Length; i++)
            {
                string folderToCheck = string.Join("/", folders.Take(i + 1));
                string parentFolder = string.Join("/", folders.Take(i));

                if (!AssetDatabase.IsValidFolder(folderToCheck))
                {
                    AssetDatabase.CreateFolder(parentFolder, folders[i]);
                }
            }
        }


        static IEnumerable<ISingletonScriptableObject> GetSingletons()
        {
            // get singleton types

            List<Type> validTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => !t.IsGenericType)
                .Where(t => t.GetInterfaces().Contains(typeof(ISingletonScriptableObject)))
                .ToList();

            // check

            foreach (Type type in validTypes)
            {

                // all instances available with the same type

                UnityEngine.Object[] allResources = Resources.LoadAll(string.Empty, type);

                ScriptableObject so = null;

                if (allResources.Length == 1)
                {
                    so = (ScriptableObject)allResources[0];

                    yield return (ISingletonScriptableObject)so;

                }

            }
        }

        private static void EditorApplicationPlayModeStateChanged(PlayModeStateChange obj)
        {

            if (obj == PlayModeStateChange.EnteredEditMode)
            {

                foreach (ISingletonScriptableObject item in GetSingletons())
                {
                    item.OnGameQuit();
                }
            }
        }

#endif
    }
}

