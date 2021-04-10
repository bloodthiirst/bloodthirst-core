using Bloodthirst.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneLoadHelper : EditorWindow
{

    private const string USS_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Load Helper/SceneLoadHelper.uss";
    private const string UXML_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "Scene Load Helper/SceneLoadHelper.uxml";

    [MenuItem("Bloodthirst Tools/Scene Management/Scene Load Helper Window")]
    public static void ShowExample()
    {
        SceneLoadHelper wnd = GetWindow<SceneLoadHelper>();
        wnd.titleContent = new GUIContent("SceneLoadHelper");
    }

    VisualElement root;

    VisualElement RowsContainer => root.Q<VisualElement>(name = nameof(RowsContainer));


    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        root = rootVisualElement;


        // Import UXML
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

        VisualElement labelFromUXML = visualTree.CloneTree();

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

        root.Add(labelFromUXML);
        root.styleSheets.Add(styleSheet);
        root.styleSheets.Add(EditorConsts.GlobalStyleSheet);

        BootstrapWindow();

    }

    public void RefreshWindow()
    {
        BootstrapWindow();
    }

    private void BootstrapWindow()
    {
        RowsContainer.Clear();

        // create a button for each scene

        LoadScenesIndividually();

        LoadAllSceneAdditively();

        PlayWithAllScenes();
    }

    private void PlayWithAllScenes()
    {
        VisualElement row = CreateRow();

        Button btn = new Button();

        btn.AddToClassList("btn");

        btn.AddToClassList("flex-grow");
        btn.AddToClassList("big-btn");

        btn.text = "Play with all scenes loaded";

        btn.clickable.clicked += () =>
        {
            OpenAllScenes();
            EditorApplication.isPlaying = true;
        };

        row.Add(btn);


    }

    private void LoadAllSceneAdditively()
    {

        VisualElement row = CreateRow();

        Button btn = new Button();

        btn.AddToClassList("btn");

        btn.AddToClassList("flex-grow");

        btn.AddToClassList("big-btn");

        btn.text = "Load All Scenes Additively";

        btn.clickable.clicked += () => { OpenAllScenes(); };

        row.Add(btn);



    }

    private void OpenAllScenes()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetSceneByBuildIndex(i).isLoaded)
            {
                continue;
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(i), UnityEditor.SceneManagement.OpenSceneMode.Additive);
        }
    }

    private void LoadScenesIndividually()
    {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            VisualElement row = CreateRow();

            Button btn = new Button();

            btn.AddToClassList("btn");

            btn.AddToClassList("flex-grow");

            string scenePath = EditorBuildSettings.scenes[i].path;

            string sceneName = scenePath.Remove(scenePath.Length - 6).Split('/').Last();

            btn.text = "Load Scene : " + sceneName;

            btn.clickable.clicked += () => { OpenScneByIndex(scenePath); };

            row.Add(btn);
        }
    }

    private void OpenScneByIndex(string scenePath)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);

        if (!UnityEditor.SceneManagement.EditorSceneManager.GetSceneByBuildIndex(buildIndex).isLoaded)
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);


        int scenesCount = UnityEditor.SceneManagement.EditorSceneManager.sceneCount;

        for (int i = scenesCount - 1; i >= 0; i--)
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i).buildIndex == buildIndex)
                continue;

            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i), true);
        }


    }

    VisualElement CreateRow()
    {

        VisualElement row = new VisualElement();
        row.AddToClassList("row");

        RowsContainer.Add(row);

        return row;
    }
}