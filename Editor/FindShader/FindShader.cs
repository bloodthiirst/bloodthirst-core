using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using Bloodthirst.Editor;
using System;
using UnityEditor.UIElements;
using System.Linq;
using Sirenix.Utilities;

public class FindShader : EditorWindow
{
    private struct SearchEnty
    {
        public Material Material { get; set; }
    }

    private const string FOLDER_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/FindShader";
    private const string UXML_PATH = FOLDER_PATH + "/" + nameof(FindShader) + ".uxml";
    private const string USS_PATH = FOLDER_PATH + "/" + nameof(FindShader) + ".uss";

    [MenuItem("Bloodthirst Tools/FindShader")]
    public static void ShowExample()
    {
        FindShader wnd = GetWindow<FindShader>();
        wnd.titleContent = new GUIContent("FindShader");
    }

    private VisualElement Root { get; set; }

    private TextField SearchTextField => Root.Q<TextField>(nameof(SearchTextField));
    private ObjectField ShaderObjectField => Root.Q<ObjectField>(nameof(ShaderObjectField));
    private Button SearchBtn => Root.Q<Button>(nameof(SearchBtn));
    private VisualElement ResultListViewContainer => Root.Q<VisualElement>(nameof(ResultListViewContainer));
    private ListView ResultListView;
    private List<SearchEnty> searchEnties = new List<SearchEnty>();

    private EditorCoroutine loadCrtHandle;

    private List<IEditorTask<FindShader>> PostLoadTasks { get; set; } = new List<IEditorTask<FindShader>>();

    public void CreateGUI()
    {
        Root = rootVisualElement;

        LoadUI();

        loadCrtHandle = EditorCoroutineUtility.StartCoroutine(CrtPostLoad(), this);

        ListenUI();
    }
    private void LoadUI()
    {
        // Import UXML
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

        visualTree.CloneTree(Root);

        Root.styleSheets.Add(styleSheet);
        if (!Root.styleSheets.Contains(EditorConsts.GlobalStyleSheet))
        {
            Root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
        }

        ShaderObjectField.objectType = typeof(Shader);
        ResultListView = new ListView(searchEnties, 30, MakeElement, BindElement);
        ResultListViewContainer.Add(ResultListView);
    }
    VisualElement MakeElement()
    {
        VisualElement ve = new VisualElement();

        ObjectField matField = new ObjectField("Material");
        matField.objectType = typeof(Material);
        matField.name = "Material";

        ve.Add(matField);

        return ve;
    }
    void BindElement(VisualElement ve, int index)
    {
        ObjectField matField = ve.Q<ObjectField>("Material");

        matField.value = searchEnties[index].Material;
    }

    private void ListenUI()
    {
        SearchBtn.clickable.clicked += HandleBtnClicked;
    }

    private void UnlistenUI()
    {
        SearchBtn.clickable.clicked -= HandleBtnClicked;
    }

    private void HandleBtnClicked()
    {
        searchEnties.Clear();

        ShaderObjectField.value = null;
        Shader targetShader = (Shader)ShaderObjectField.value;


        if (targetShader == null && !string.IsNullOrEmpty(SearchTextField.value))
        {
            List<Shader> allShader = EditorUtils.FindAssets<Shader>();

            string normalizedSearchTxt = SearchTextField.value.ToLower();

            for (int i = 0; i < allShader.Count; ++i)
            {
                Shader curr = allShader[i];

                ShaderImporter shaderImporter = (ShaderImporter)ShaderImporter.GetAtPath(AssetDatabase.GetAssetPath(curr));
                ShaderInfo shaderInfo = ShaderUtil.GetShaderInfo(curr);
                ShaderData shaderData = ShaderUtil.GetShaderData(curr);

                bool isFound =
                    shaderImporter.assetPath.ToLower().Contains(normalizedSearchTxt) ||
                    shaderInfo.name.ToLower().Contains(normalizedSearchTxt);

                if (!isFound)
                    continue;

                targetShader = curr;
                ShaderObjectField.value = curr;
                break;
            }
        }

        if (targetShader == null)
        {
            EditorUtility.DisplayDialog("Error", "Shader not found", "Ok");
            return;
        }


        List<Material> allMaterials = EditorUtils.FindAssets<Material>();


        for (int i = 0; i < allMaterials.Count; i++)
        {
            Material m = allMaterials[i];

            if (m.shader != targetShader)
                continue;

            SearchEnty searchEnty = new SearchEnty();
            searchEnty.Material = m;
            searchEnties.Add(searchEnty);
        }

        ResultListView.RefreshItems();
    }



    public void AddPostLoadTask(IEditorTask<FindShader> editorTask)
    {
        PostLoadTasks.Add(editorTask);
    }

    private IEnumerator CrtPostLoad()
    {
        // wait until layout it built
        yield return new WaitUntil(Root.IsLayoutBuilt);

        // execute post load tasks
        for (int i = PostLoadTasks.Count - 1; i >= 0; i--)
        {
            IEditorTask<FindShader> t = PostLoadTasks[i];

            t.Execute(this);

            PostLoadTasks.RemoveAt(i);
        }

        loadCrtHandle = null;
    }

    private void OnDestroy()
    {
        UnlistenUI();
        PostLoadTasks.Clear();
        loadCrtHandle = null;
    }

}