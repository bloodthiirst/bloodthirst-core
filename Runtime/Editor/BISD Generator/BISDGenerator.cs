using Bloodthirst.Core.Utils;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.BISD.Editor
{
    public class BISDGenerator : EditorWindow
    {
        private const string BEHAVIOUR_TEMPALTE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/Template.Behaviour.cs.txt";

        private const string INSTANCE_TEMPALTE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/Template.Instance.cs.txt";

        private const string STATE_TEMPALTE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/Template.State.cs.txt";

        private const string DATA_TEMPALTE = "Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/Template.Data.cs.txt";

        private const string REPLACE_KEYWORD = "[MODELNAME]";

        [MenuItem("Bloodthirst Tools/BISDGenerator")]
        public static void ShowWindow()
        {
            BISDGenerator wnd = GetWindow<BISDGenerator>();
            wnd.titleContent = new GUIContent("BISDGenerator");
        }

        [MenuItem("Assets/BISD Generator")]
        public static void AssetMenu()
        {
            ShowWindow();
        }

        public Button GenerateBtn => rootVisualElement.Q<Button>(name = "GenerateBtn");

        public TextField ModelName => rootVisualElement.Q<TextField>(name = "ModelName");

        public Label PathPreview => rootVisualElement.Q<Label>(name = "PathPreview");

        public void OnEnable()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/BISDGenerator.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            root.Add(labelFromUXML);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.bloodthirst.bloodthirst-core/Runtime/Editor/BISD Generator/BISDGenerator.uss");

            SetupElements();
        }

        private void SetupElements()
        {
            // text field
            ModelName.RegisterValueChangedCallback(OnModelNameChanged);

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

            GenerateFiles(currentFolder, ModelName.value);
        }

        private void GenerateFiles(string currentFolder, string modelName)
        {
            string FolderName = modelName + "Model";

            string folderGUID = AssetDatabase.CreateFolder(currentFolder, FolderName);

            string relativePath = AssetDatabase.GUIDToAssetPath(folderGUID)
                        + "/"
                        + modelName;

            string finalPath = EditorUtils.PathToProject
                             + relativePath;



            Debug.Log(finalPath);

            File.WriteAllText(finalPath + "Behaviour.cs", AssetDatabase.LoadAssetAtPath<TextAsset>(BEHAVIOUR_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName));
            File.WriteAllText(finalPath + "Instance.cs", AssetDatabase.LoadAssetAtPath<TextAsset>(INSTANCE_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName));
            File.WriteAllText(finalPath + "State.cs", AssetDatabase.LoadAssetAtPath<TextAsset>(STATE_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName));
            File.WriteAllText(finalPath + "Data.cs", AssetDatabase.LoadAssetAtPath<TextAsset>(DATA_TEMPALTE).text.Replace(REPLACE_KEYWORD, modelName));

            AssetDatabase.ImportAsset(relativePath + "Behaviour.cs");
            AssetDatabase.ImportAsset(relativePath + "Instance.cs");
            AssetDatabase.ImportAsset(relativePath + "State.cs");
            AssetDatabase.ImportAsset(relativePath + "Data.cs");

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


                string FolderName = evt.newValue + "Model";

                string relativePath = GetSelectedPathOrFallback()
                            + "/"
                            + FolderName
                            + "/"
                            + evt.newValue;

                PathPreview.text = relativePath;

            }
        }
    }
}