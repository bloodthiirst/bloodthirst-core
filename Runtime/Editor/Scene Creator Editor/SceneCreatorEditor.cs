using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using UnityEditor.Callbacks;
using System.Linq;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.PersistantAsset;
using System.Collections.Generic;

public class SceneCreatorEditor : EditorWindow
{
    private const string SCENE_MANAGER_TEMPALTE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Scene Creator Editor/Template.SceneManager.cs.txt";

    private const string REPLACE_KEYWORD = "[SCENENAME]";

    [DidReloadScripts(SingletonScriptableObjectInit.SCENE_CREATOR)]
    public static void OnReloadScripts()
    {
        bool isNewSceneAdded = false;

        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            Scene scene = EditorSceneManager.GetSceneAt(i);

            /*
            if (scene.buildIndex == -1)
            {
                continue;
            }
            */
            bool hasManager = scene.GetRootGameObjects().FirstOrDefault(g => g.name.Equals("Scene Manager")) != null;

            // if scene does not have a manager


            if (!hasManager)
            {

                // search for the manager type

                Type sceneManagerType = null;

                IEnumerable<Type> allTypes = AppDomain.CurrentDomain
                                            .GetAssemblies()
                                            .SelectMany(asm => asm.GetTypes())
                                            .Where(t => t.Name.Contains(scene.name + "Manager"));

                foreach (Type t in allTypes)
                {
                    if (t.Name.Contains(scene.name + "Manager"))
                    {
                        sceneManagerType = t;
                        break;
                    }
                }

                // if scene doesnt have a maanger then 


                if (sceneManagerType == null)
                {
                    //Debug.LogError("Couldn't find the right scene maanger for the scene : " + scene.name);
                    continue;
                }

                // create the manager gameObject

                GameObject sceneManager = new GameObject("Scene Manager");

                EditorSceneManager.MoveGameObjectToScene(sceneManager, scene);

                // add the scene manager component

                sceneManager.AddComponent(sceneManagerType);

                EditorSceneManager.SaveScene(scene);

                isNewSceneAdded = true;
            }
        }

        // if no scene has been added then exit

        if (!isNewSceneAdded)
        {
            return;
        }

        // refresh the scene list data

        ScenesListData scenesListData = Resources.Load<ScenesListData>(ScenesListData.AssetPath);

        if (scenesListData == null)
        {
            Debug.LogError("Scene list data ScriptableObject was not found !!!");
            return;
        }

        scenesListData.LoadAllScenesAvailable();

        if (HasOpenInstances<SceneLoadHelper>())
        {
            GetWindow<SceneLoadHelper>().RefreshWindow();
        }


    }

    [MenuItem("Bloodthirst Tools/Scene Creator Editor")]
    public static void ShowWindow()
    {
        SceneCreatorEditor wnd = GetWindow<SceneCreatorEditor>();
        wnd.titleContent = new GUIContent("SceneCreatorEditor");
    }

    [MenuItem("Assets/Scene Creator Editor")]
    public static void AssetMenu()
    {
        ShowWindow();
    }

    VisualElement root;

    public Button GenerateBtn => rootVisualElement.Q<Button>(name = nameof(GenerateBtn));

    public TextField SceneName => rootVisualElement.Q<TextField>(name = nameof(SceneName));

    public Label PathPreview => rootVisualElement.Q<Label>(name = nameof(PathPreview));

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;

        // Import UXML
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Scene Creator Editor/SceneCreatorEditor.uxml");

        VisualElement labelFromUXML = visualTree.CloneTree();

        // A stylesheet can be added to a VisualElement.
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Scene Creator Editor/SceneCreatorEditor.uss");

        root.Add(labelFromUXML);
        root.styleSheets.Add(styleSheet);

        SetupElements();


        SceneName.focusable = true;
        SceneName.Focus();

    }

    private void SetupElements()
    {
        // text field
        SceneName.RegisterValueChangedCallback(OnModelNameChanged);

        //generate btn
        GenerateBtn.clickable.clicked += OnGenerateBtnClicked;
        GenerateBtn.SetEnabled(false);
    }

    /// <summary>
    /// Retrieves selected folder on Project view.
    /// </summary>
    /// <returns></returns>
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    private void OnGenerateBtnClicked()
    {
        string currentFolder = GetSelectedPathOrFallback();

        GenerateFiles(currentFolder, SceneName.value);
    }

    private void GenerateFiles(string currentFolder, string sceneName)
    {
        string FolderName = sceneName + "Scene";

        string folderGUID = AssetDatabase.CreateFolder(currentFolder, FolderName);

        string relativePath = AssetDatabase.GUIDToAssetPath(folderGUID)
                    + "/"
                    + sceneName;

        string finalPath = Application.dataPath.TrimEnd("Assets".ToCharArray())
                         + relativePath;



        string fileContent = AssetDatabase.LoadAssetAtPath<TextAsset>(SCENE_MANAGER_TEMPALTE).text.Replace(REPLACE_KEYWORD, sceneName);

        // create file and write its content
        File.WriteAllText(finalPath + "SceneManager.cs", fileContent);

        // create scene asset

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = sceneName + "Scene";

        EditorSceneManager.SaveScene(scene, relativePath + "Scene.unity");

        AssetDatabase.ImportAsset(finalPath + "SceneManager.cs");

        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    private void OnModelNameChanged(ChangeEvent<string> evt)
    {
        if (string.IsNullOrEmpty(evt.newValue) || string.IsNullOrWhiteSpace(evt.newValue))
        {
            GenerateBtn.SetEnabled(false);
            PathPreview.text = "(Invalid)";
        }
        else
        {
            GenerateBtn.SetEnabled(true);


            string FolderName = evt.newValue + "Scene";

            string relativePath = GetSelectedPathOrFallback()
                        + "/"
                        + FolderName
                        + "/"
                        + evt.newValue;

            PathPreview.text = relativePath;

        }
    }
}