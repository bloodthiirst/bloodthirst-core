using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneCreatorEditor : EditorWindow
{
    private const string SCENE_MANAGER_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Creator Editor/Template.SceneManager.cs.txt";

    private const string REPLACE_KEYWORD = "[SCENENAME]";

    [MenuItem("Bloodthirst Tools/Scene Management/Refresh Scene Setup")]
    private static void SetupTheSceneManagers()
    {
        EditorApplication.update -= SetupTheSceneManagers;

        bool isNewSceneAdded = false;

        // save open scenes

        List<Scene> savedOpenScenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            savedOpenScenes.Add(scene);
        }

        // get all scenes and open them
        List<Scene> allScenes = new List<Scene>();

        List<string> allScenePaths = EditorUtils.GetAllScenePathsInProject();
        
        for (int i = 0; i < allScenePaths.Count; i++)
        {
            string scenePath = allScenePaths[i];

            if (!scenePath.StartsWith("Assets/Scenes"))
                continue;

            Scene s = SceneManager.GetSceneByPath(scenePath);

            if (!s.IsValid())
            {
                s = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
            allScenes.Add(s);
        }


        for (int i = 0; i < allScenes.Count; i++)
        {
            Scene scene = allScenes[i];

            if (!scene.IsValid())
            {
                Debug.LogError($"the scene '{scene.name}' is invalid");
                continue;
            }
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
                Type sceneManagerAdapter = TypeUtils.AllTypes.FirstOrDefault(t => t.Name == "SceneInstanceManagerAdapter");

                IEnumerable<Type> allTypes = TypeUtils.AllTypes
                                            .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t))
                                            .Where(t => t.Name.Contains(scene.name + "Manager"));

                foreach (Type t in allTypes)
                {
                    if (t.Name.Contains(scene.name + "Manager"))
                    {
                        sceneManagerType = t;
                        break;
                    }
                }

                // if scene doesnt have a manager then

                if (sceneManagerType == null)
                {
                    //Debug.LogError("Couldn't find the right scene manager for the scene : " + scene.name);
                    continue;
                }

                // create the manager gameObject

                GameObject sceneManager = new GameObject("Scene Manager");

                UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(sceneManager, scene);

                // add the scene manager component

                sceneManager.AddComponent(sceneManagerType);
                sceneManager.AddComponent(sceneManagerAdapter);

                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);

                isNewSceneAdded = true;
            }
        }

        // close scene and leave the ones previously open

        foreach (string scenePath in EditorUtils.GetAllScenePathsInProject())
        {
            Scene s = SceneManager.GetSceneByPath(scenePath);

            if (!savedOpenScenes.Contains(s))
            {
                EditorSceneManager.CloseScene(s, true);
            }
        }

        // if no scene has been added then exit

        if (!isNewSceneAdded)
        {
            return;
        }

        // refresh the scene list data

        //ScenesListData scenesListData = Resources.Load<ScenesListData>(ScenesListData.AssetPath);
        ScenesListData scenesListData = ScenesListData.Instance;

        if (scenesListData == null)
        {
            Debug.LogError("Scene list data ScriptableObject was not found !!!");
            return;
        }

        // reload the scenes list in the scene data scriptable object

        scenesListData.InitializeScenes();

        if (HasOpenInstances<SceneLoadHelper>())
        {
            GetWindow<SceneLoadHelper>().RefreshWindow();
        }
    }

    [MenuItem("Bloodthirst Tools/Scene Management/Scene Creator Editor")]
    public static void ShowWindow()
    {
        SceneCreatorEditor wnd = GetWindow<SceneCreatorEditor>();
        wnd.titleContent = new GUIContent("SceneCreatorEditor");
    }

    [MenuItem("Assets/Scene Management/Scene Creator Editor")]
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
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH  + "Scene Creator Editor/SceneCreatorEditor.uxml");

        VisualElement labelFromUXML = visualTree.CloneTree();

        // A stylesheet can be added to a VisualElement.
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Creator Editor/SceneCreatorEditor.uss");

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

        CreateNewScene(currentFolder, SceneName.value);
    }

    /// <summary>
    /// <para>Create a scene using the convention of BloodthirstCore.</para>
    /// <para>This will generate a scene + the appropriate <see cref="SceneInstanceManager{T}"/> that will be added to the scene during the next domain reload.</para>
    /// </summary>
    /// <param name="currentFolder"></param>
    /// <param name="sceneName"></param>
    public static void CreateNewScene(string currentFolder, string sceneName)
    {
        string fullSceneName = sceneName + "Scene";

        // folder containing the scene related files
        string relativePath = $"{currentFolder}/{fullSceneName}";

        EditorUtils.CreateFoldersFromPath(relativePath);

        string finalPath = $"{EditorUtils.PathToProject}/{relativePath}/{sceneName}";



        string fileContent = AssetDatabase.LoadAssetAtPath<TextAsset>(SCENE_MANAGER_TEMPALTE).text.Replace(REPLACE_KEYWORD, sceneName);

        // create file and write its content
        File.WriteAllText(finalPath + "SceneManager.cs", fileContent);

        // create scene asset

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = sceneName + "Scene";

        EditorSceneManager.SaveScene(scene, $"{relativePath}/{fullSceneName}.unity");

        EditorSceneManager.CloseScene(scene, true);

        AssetDatabase.ImportAsset($"{relativePath} ", ImportAssetOptions.ForceUpdate);

        AssetDatabase.SaveAssets();


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