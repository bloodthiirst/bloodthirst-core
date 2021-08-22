using Bloodthirst.Core.BISD.CodeGeneration;
using Bloodthirst.Core.BISD.Editor.Commands;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.Commands;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using ICodeGenerator = Bloodthirst.Core.BISD.CodeGeneration.ICodeGenerator;

namespace Bloodthirst.Core.BISD.Editor
{
    public class BISDGenerator : EditorWindow
    {


        private const string UXML_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/BISDGenerator.uxml";

        private const string UXML_INFO_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/BISDInfo.uxml";

        private const string USS_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/BISDGenerator.uss";


        [MenuItem("Bloodthirst Tools/BISD Pattern/BISD Generator")]
        public static void ShowWindow()
        {
            BISDGenerator wnd = GetWindow<BISDGenerator>();
            wnd.titleContent = new GUIContent("BISD Generator");
        }

        [MenuItem("Assets/Create/BISD Generator")]
        public static void AssetMenu()
        {
            ShowWindow();
        }

        [DidReloadScripts]
        private static void OnReload()
        {
            if (!HasOpenInstances<BISDGenerator>())
                return;

            GetWindow<BISDGenerator>().Refresh();
        }

        public Button GenerateBtn => rootVisualElement.Q<Button>(name = "GenerateBtn");
        public VisualElement BISDList => rootVisualElement.Q<VisualElement>(name = "BISDList");

        public TextField ModelName => rootVisualElement.Q<TextField>(name = "ModelName");

        public Label PathPreview => rootVisualElement.Q<Label>(name = "PathPreview");

        public void OnEnable()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            VisualElement template = visualTree.CloneTree();
            root.Add(template);

            template.AddToClassList("w-100");
            template.AddToClassList("h-100");

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            root.styleSheets.Add(styleSheet);
            root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
            SetupElements();
        }

        private void SetupElements()
        {
            // text field
            ModelName.RegisterValueChangedCallback(OnModelNameChanged);

            //generate btn
            GenerateBtn.clickable.clicked += OnGenerateBtnClicked;
            GenerateBtn.SetEnabled(false);

            ScrollView scroll = new ScrollView(ScrollViewMode.Vertical);

            BISDList.Clear();
            BISDList.Add(scroll);

            // TODO : do this on other thread
            // code generators
            List<ICodeGenerator> codeGenerators = new List<ICodeGenerator>()
            {
                new ObservableFieldsCodeGenerator(),
                new GameStateCodeGenerator(),
                new LoadSaveHandlerCodeGenerator()
            };

            ExtractBISDInfoCommand cmd = new ExtractBISDInfoCommand();

            // get models info
            Dictionary<string, BISDInfoContainer> typeList = CommandManagerEditor.Run(cmd);

            
            string[] models = typeList.Keys.ToArray();

            bool dirty = false;

            int affctedModels = 0;

            // run thorugh the models to apply the changes
            foreach (string model in models)
            {
                BISDInfoContainer typeInfo = typeList[model];

                bool modeldirty = false;

                foreach (ICodeGenerator generator in codeGenerators)
                {
                    if (generator.ShouldInject(typeInfo))
                    {
                        dirty = true;
                        modeldirty = true;
                        generator.InjectGeneratedCode(typeInfo);
                    }
                }

                if (modeldirty)
                {
                    affctedModels++;
                }
            }

            Debug.Log($"BISD affected models : {affctedModels}");

            if (!dirty)
                return;


            // save the changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            for (int i = 0; i< 5; i++)
            {
                VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_INFO_PATH);
                VisualElement template = visualTree.CloneTree().Children().First();

                scroll.Add(template);
            }

            Refresh();
        }

        /// <summary>
        /// Retrieves selected folder on Project view.
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedPathOrFallback()
        {

            string defaultPath = EditorUtils.CurrentProjectWindowPath();
            
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    return path = Path.GetDirectoryName(path);
                }
            }


            return defaultPath;
        }

        private void OnGenerateBtnClicked()
        {
            string currentFolder = GetSelectedPathOrFallback();

            GenerateFiles(currentFolder, ModelName.value);
        }

        private void GenerateFiles(string currentFolder, string modelName)
        {
            string FolderName = modelName + "Model";

            string folderGUID = AssetDatabase.CreateFolder(currentFolder, FolderName);

            string relativePath = AssetDatabase.GUIDToAssetPath(folderGUID) + "/";

            CommandManagerEditor.Run(new CreateBehaviourFileCommand(modelName, relativePath));
            CommandManagerEditor.Run(new CreateInstanceFileCommand(modelName, relativePath));
            CommandManagerEditor.Run(new CreateStateFileCommand(modelName, relativePath));
            CommandManagerEditor.Run(new CreateDataFileCommand(modelName, relativePath));
            CommandManagerEditor.Run(new CreateGameDataFileCommand(modelName, relativePath));
            CommandManagerEditor.Run(new CreateLoadSaveHandlerFileCommand(modelName, relativePath));
        }

        private void OnModelNameChanged(ChangeEvent<string> evt)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (string.IsNullOrEmpty(ModelName.text) || string.IsNullOrWhiteSpace(ModelName.text))
            {
                GenerateBtn.SetEnabled(false);
                PathPreview.text = "(Invalid Model Name)";
            }
            else
            {
                GenerateBtn.SetEnabled(true);


                string FolderName = ModelName.text + "Model";

                string relativePath = GetSelectedPathOrFallback()
                            + "/"
                            + FolderName;

                PathPreview.text = relativePath;

            }
        }
    }
}