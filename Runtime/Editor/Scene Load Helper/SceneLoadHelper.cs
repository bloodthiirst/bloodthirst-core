using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

public class SceneLoadHelper : EditorWindow
{

    [MenuItem("Bloodthirst Tools/Scene Load Helper")]
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
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Scene Load Helper/SceneLoadHelper.uxml");

        VisualElement labelFromUXML = visualTree.CloneTree();

        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/Scene Load Helper/SceneLoadHelper.uss");

        root.styleSheets.Add(styleSheet);

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

        btn.AddToClassList("h-50");

        btn.text = "Play with all scenes loaded";

        btn.clickable.clicked += () => {
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

        btn.AddToClassList("h-50");

        btn.text = "Load All Scenes Additively";

        btn.clickable.clicked += () => { OpenAllScenes(); };

        row.Add(btn);


        
    }

    private void OpenAllScenes()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if (EditorSceneManager.GetSceneByBuildIndex(i).isLoaded)
            {
                continue;
            }

            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(i), OpenSceneMode.Additive);
        }
    }

    private void LoadScenesIndividually()
    {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            VisualElement row = CreateRow();

            Button btn = new Button();

            btn.AddToClassList("btn");

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

        if (!EditorSceneManager.GetSceneByBuildIndex(buildIndex).isLoaded)
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);


        int scenesCount = EditorSceneManager.sceneCount;

        for (int i = scenesCount - 1; i >= 0; i--)
        {
            if (EditorSceneManager.GetSceneAt(i).buildIndex == buildIndex)
                continue;

            EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(i), true);
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