using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneCreatorEditor : EditorWindow
{
    private const string SCENE_MANAGER_TEMPALTE = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Creator Editor/Template.SceneManager.cs.txt";
    private const string SCENE_CREATOR_DATA = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Creator Editor/SceneCreatorData.asset";

    private const string REPLACE_KEYWORD = "[SCENENAME]";

    private static IReadOnlyList<Type> SceneManagerTypes;

    private static Type SceneManagerAdapterType;

    static SceneCreatorEditor()
    {
        // get SceneManagerAdapterType
        SceneManagerAdapterType = TypeUtils.AllTypes.FirstOrDefault(t => t.Name == "SceneInstanceManagerAdapter");

        // get all SceneManger types
        SceneManagerTypes = new List<Type>(TypeUtils.AllTypes
                                    .Where(t => t.IsClass)
                                    .Where(t => !t.IsAbstract)
                                    .Where(t => TypeUtils.IsSubTypeOf(t, typeof(MonoBehaviour)) && TypeUtils.IsSubTypeOf(t, typeof(ISceneInstanceManager))));
    }

    private static SceneCreatorData data;
    private static SceneCreatorData Data
    {
        get
        {
            if (data == null)
            {
                data = AssetDatabase.LoadAssetAtPath<SceneCreatorData>(SCENE_CREATOR_DATA);
            }

            if (data == null)
            {
                AssetDatabase.CreateAsset(new SceneCreatorData(), SCENE_CREATOR_DATA);
                data = AssetDatabase.LoadAssetAtPath<SceneCreatorData>(SCENE_CREATOR_DATA);
            }

            return data;
        }
    }

    [InitializeOnLoadMethod]
    public static void Init()
    {
        for (int i = Data.pendingScenesToProcess.Count - 1; i >= 0; i--)
        {
            string s = Data.pendingScenesToProcess[i];

            if (CheckForSceneManager(s))
            {
                Data.pendingScenesToProcess.RemoveAt(i);
            }
        }
    }

    [MenuItem("Bloodthirst Tools/Scene Management/Refresh Scene Setup")]
    private static void SetupTheSceneManagers()
    {
        // save open scenes
        List<Scene> savedOpenScenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            savedOpenScenes.Add(scene);
        }

        List<string> allScenePaths = EditorUtils.GetAllScenePathsInProject();

        for (int i = 0; i < allScenePaths.Count; i++)
        {
            string scenePath = allScenePaths[i];

            CheckForSceneManager(scenePath);
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
            GetWindow<SceneLoadHelper>().RedrawUI();
        }
    }

    /// <summary>
    /// Checks whether the scene is well setup and tries to setup it up
    /// </summary>
    /// <param name="scenePath"></param>
    /// <returns>true if scene is well setup , false otherwise</returns>
    private static bool CheckForSceneManager(string scenePath)
    {
        if (!scenePath.StartsWith("Assets/Scenes"))
            return false;

        bool needToClose = false;

        Scene currScene = SceneManager.GetSceneByPath(scenePath);

        if (!currScene.isLoaded)
        {
            currScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            needToClose = true;
        }

        if (!HasManagerType(currScene, out Type managerType))
        {
            if (needToClose)
            {
                EditorSceneManager.CloseScene(currScene, true);
            }
            return false;
        }

        if (HasManager(currScene, managerType))
        {
            if (needToClose)
            {
                EditorSceneManager.CloseScene(currScene, true);
            }
            return true;
        }

        AddManager(currScene, managerType);

        if (needToClose)
        {
            EditorSceneManager.CloseScene(currScene, true);
        }

        return true;
    }

    private static bool HasManager(Scene scene, Type managerType)
    {
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            Component managerCmp = go.GetComponentInChildren(managerType);

            if (managerCmp != null)
                return true;
        }

        return false;
    }

    private static bool HasManagerType(Scene scene, out Type managerType)
    {
        foreach (Type t in SceneManagerTypes)
        {
            if (t.Name == (scene.name + "SceneManager"))
            {
                managerType = t;
                return true;
            }
        }

        managerType = null;
        return false;
    }

    private static void AddManager(Scene scene, Type managerType)
    {
        // create the manager gameObject
        GameObject sceneManager = new GameObject("Scene Manager");

        SceneManager.MoveGameObjectToScene(sceneManager, scene);

        // add the scene manager component
        sceneManager.AddComponent(SceneManagerAdapterType);
        sceneManager.AddComponent(managerType);

        EditorSceneManager.SaveScene(scene);
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
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Creator Editor/SceneCreatorEditor.uxml");

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
        string path = EditorUtils.GetProjectTabPath();

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
        // folder containing the scene related files
        string relativePath = $"{currentFolder}/{sceneName}";
        string scenePath = $"{relativePath}/{sceneName}.unity";
        string scriptPath = $"{relativePath}/{sceneName}SceneManager.cs";

        EditorUtils.CreateFoldersFromPath(relativePath);

        // create file and write its content
        {
            string absoluteFilePath = EditorUtils.RelativeToAbsolutePath(scriptPath);
            string fileContent = AssetDatabase.LoadAssetAtPath<TextAsset>(SCENE_MANAGER_TEMPALTE).text.Replace(REPLACE_KEYWORD, sceneName);
            File.WriteAllText(absoluteFilePath, fileContent);
        }

        // create scene asset
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            scene.name = sceneName;

            /*
            Scene openScene = EditorSceneManager.GetSceneByPath(scenePath);

            if(openScene.isLoaded)
            {
                EditorSceneManager.CloseScene(openScene, true);
            }
            */
            EditorSceneManager.SaveScene(scene, scenePath, true);
            EditorSceneManager.CloseScene(scene, true);
        }

        Data.pendingScenesToProcess.Add(scenePath);

        AssetDatabase.ImportAsset(scenePath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(scriptPath, ImportAssetOptions.ForceUpdate);
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


            string FolderName = evt.newValue;

            string relativePath = GetSelectedPathOrFallback()
                        + "/"
                        + FolderName
                        + "/"
                        + evt.newValue;

            PathPreview.text = relativePath;

        }
    }
}