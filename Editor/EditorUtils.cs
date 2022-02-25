#if UNITY_EDITOR
using Bloodthirst.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.Utils
{
    /// <summary>
    /// an extension class containing helper methods for types / reflection
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// Clears the console in the editor
        /// </summary>
        public static void ClearConsole()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        public static void Display( this VisualElement visualElement, bool show )
        {
            DisplayStyle style  = show ? DisplayStyle.Flex : DisplayStyle.None;
            visualElement.style.display = style;
        }

        public static string CurrentProjectWindowPath()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();

            return pathToCurrentFolder;
        }

        /// <summary>
        /// <para>Create a folder based on path</para>
        /// <para>example : "Assets/Resources/Foo/Bar"</para>
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateFoldersFromPath(string folderPath)
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

        private static string pathToProject;

        /// <summary>
        ///<para>Path to project (without the Asset folder)</para>
        ///<para>Example : C:/UnityProjects/[ProjectName]</para>
        /// </summary>
        public static string PathToProject
        {
            get
            {
                if (string.IsNullOrEmpty(pathToProject))
                {
                    pathToProject = Application.dataPath.TrimEnd("Assets".ToCharArray());
                }

                return pathToProject;
            }
        }

        /// <summary>
        /// Get all the text assets in the project , this includes scripts
        /// </summary>
        /// <returns></returns>
        public static List<MonoScript> FindScriptAssets()
        {
            List<MonoScript> assets = new List<MonoScript>();
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);

                MonoScript asset = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        /// <summary>
        /// Get all scene in the project (open AND closed)
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllScenePathsInProject()
        {
            // load scene assets
            string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");

            List<string> editorBuildSettingsScenes = new List<string>();

            foreach (string sceneGUID in scenesGUIDs)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);

                // if scene is valid add it to the scenes list in build settings
                editorBuildSettingsScenes.Add(scenePath);
            }

            return editorBuildSettingsScenes;
        }


        /// <summary>
        /// Get a path to an asset placed in a resource folder
        /// </summary>
        /// <param name="poolablePrefab"></param>
        /// <returns></returns>
        public static string GetResourcesPath(UnityEngine.Object poolablePrefab)
        {
            var path = AssetDatabase.GetAssetPath(poolablePrefab);

            if (string.IsNullOrEmpty(path))
                return null;

            int index = path.LastIndexOf("Resources/", path.Length - 1);

            path = path.Substring(index + "Resources/".Length);

            if (index == -1)
                return path;

            int extensionDot = path.LastIndexOf(".", path.Length - 1);

            if (extensionDot == -1)
                return path;

            return path.Substring(0, extensionDot);

        }
    }
}
#endif
