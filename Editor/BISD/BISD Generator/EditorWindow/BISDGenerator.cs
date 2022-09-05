using Bloodthirst.Core.BISD.CodeGeneration;
using Bloodthirst.Core.BISD.Editor.Commands;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.Commands;
using Bloodthirst.System.CommandSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.BISD.Editor
{
    public class BISDGenerator : EditorWindow
    {
        private const string UXML_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/EditorWindow/BISDGenerator.uxml";

        private const string USS_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/EditorWindow/BISDGenerator.uss";

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

            GetWindow<BISDGenerator>().RefreshGenerateModel();
        }

        public Button GenerateBtn => rootVisualElement.Q<Button>(name = "GenerateBtn");
        public Button RefreshBtn => rootVisualElement.Q<Button>(name = "RefreshBtn");
        public Button TriggerGeneratorsBtn => rootVisualElement.Q<Button>(name = "TriggerGeneratorsBtn");
        public VisualElement BISDList => rootVisualElement.Q<VisualElement>(name = "BISDList");

        public TextField ModelName => rootVisualElement.Q<TextField>(name = "ModelName");
        public TextField SearchFilter => rootVisualElement.Q<TextField>(name = "SearchFilter");

        public Label PathPreview => rootVisualElement.Q<Label>(name = "PathPreview");

        #region loading

        public Label LoadingTask => rootVisualElement.Q<Label>(name = "LoadingTask");

        private StringBuilder sb = new StringBuilder();

        #endregion

        private ScrollView bisdInfoScrollview;

        private List<BISDInfoUI> infoUIs = new List<BISDInfoUI>();

        private Dictionary<string, BISDInfoContainer> bisdScannedData;
        public Dictionary<string, BISDInfoContainer> BISDScannedData
        {
            get => bisdScannedData;
            private set
            {
                if (bisdScannedData == value)
                    return;

                bisdScannedData = value;
                RefreshUIBISDListView(bisdScannedData);
            }
        }

        List<ICommandBase> runningBackgroundTasks = new List<ICommandBase>();
        private EditorCoroutine crtRefresh;

        public event Action<ICommandBase> OnTaskCommandAdded;

        public event Action<ICommandBase> OnTaskCommandRemoved;

        private void AddTask(ICommandBase taskCommand)
        {
            runningBackgroundTasks.Add(taskCommand);
            OnTaskCommandAdded?.Invoke(taskCommand);
        }
        private void RemoveTask(ICommandBase taskCommand)
        {
            runningBackgroundTasks.Remove(taskCommand);
            OnTaskCommandRemoved?.Invoke(taskCommand);
        }


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

            RefreshModels();
        }

        private void OnDestroy()
        {
            if (crtRefresh != null)
            {
                EditorCoroutineUtility.StopCoroutine(crtRefresh);
                crtRefresh = null;
            }

            foreach (ICommandBase cmd in runningBackgroundTasks)
            {
                cmd.Interrupt();
            }

            runningBackgroundTasks.Clear();

        }

        private void RefreshModels()
        {
            crtRefresh = EditorCoroutineUtility.StartCoroutine(CrtExtractThreaded(), this);
        }

        private IEnumerator CrtExtractThreaded()
        {

            ExtractBISDInfoCommandThreaded cmd = new ExtractBISDInfoCommandThreaded();

            bisdInfoScrollview.Clear();

            AddTask(cmd);

            // get models info in a threaded way
            CommandManagerEditor.Run(cmd);

            while ( cmd.CommandState == COMMAND_STATE.SUCCESS ||
                    cmd.CommandState == COMMAND_STATE.FAILED ||
                    cmd.CommandState == COMMAND_STATE.INTERRUPTED)
            
            {
                yield return null;
            }

            RemoveTask(cmd);

            Dictionary<string, BISDInfoContainer> res = cmd.Result;

            BISDScannedData = res;

            crtRefresh = null;

            yield break;
        }

        private void RefreshUIBISDListView(Dictionary<string, BISDInfoContainer> info)
        {
            // clear old ones
            // clear events

            for (int i = infoUIs.Count - 1; i > -1; i--)
            {
                BISDInfoUI curr = infoUIs[i];
                curr.OnCodeGenerationChanged -= HandleCodeGenerationChanged;

                infoUIs.RemoveAt(i);
            }

            bisdInfoScrollview.Clear();

            string searchTxt = SearchFilter.value.ToLower().Trim();

            if (string.IsNullOrEmpty(searchTxt))
            {
                foreach (KeyValuePair<string, BISDInfoContainer> kv in BISDScannedData)
                {
                    BISDInfoUI i = new BISDInfoUI(kv.Key, kv.Value);

                    i.OnCodeGenerationChanged += HandleCodeGenerationChanged;

                    bisdInfoScrollview.Add(i.VisualElement);
                    infoUIs.Add(i);
                }
            }
            else
            {
                foreach (KeyValuePair<string, BISDInfoContainer> kv in BISDScannedData)
                {
                    if (!kv.Key.ToLower().Contains(searchTxt))
                        continue;

                    BISDInfoUI i = new BISDInfoUI(kv.Key, kv.Value);

                    i.OnCodeGenerationChanged += HandleCodeGenerationChanged;

                    bisdInfoScrollview.Add(i.VisualElement);
                    infoUIs.Add(i);
                }
            }


        }

        private void HandleCodeGenerationChanged(BISDInfoUI ui, CodeGenerationOptionUI option)
        {

            if (option.IsSelected)
            {
                TriggerGeneratorsBtn.SetEnabled(true);
                return;
            }

            foreach (BISDInfoUI currUi in infoUIs)
            {
                foreach (CodeGenerationOptionUI op in currUi.codeGenerationOptions)
                {
                    if (op.IsSelected)
                    {
                        TriggerGeneratorsBtn.SetEnabled(true);
                        return;
                    }
                }
            }

            TriggerGeneratorsBtn.SetEnabled(false);
        }

        private void SetupElements()
        {
            // text field
            ModelName.RegisterValueChangedCallback(OnModelNameChanged);
            SearchFilter.RegisterValueChangedCallback(OnSearchFilterChanged);

            GenerateBtn.clickable.clicked += OnGenerateBtnClicked;
            RefreshBtn.clickable.clicked += OnRefreshBtnClicked;
            TriggerGeneratorsBtn.clickable.clicked += HandleCodeGenerationBtnClicked;

            TriggerGeneratorsBtn.SetEnabled(false);

            bisdInfoScrollview = new ScrollView(ScrollViewMode.Vertical);

            BISDList.Clear();
            BISDList.Add(bisdInfoScrollview);

            // tasks
            OnTaskCommandAdded += HandleTaskAdded;
            OnTaskCommandRemoved += HandleTaskRemoved;

            RefreshGenerateModel();
            RefreshTasksText();
            RefreshGenerateModel();
            RefreshButtons();
        }

        private void HandleCodeGenerationBtnClicked()
        {
            foreach (BISDInfoUI ui in infoUIs)
            {
                foreach (CodeGenerationOptionUI op in ui.codeGenerationOptions)
                {
                    if (!op.IsSelected)
                        continue;

                    CommandManagerEditor.RunInstant(new CommandExecuteCodeGenerator(ui.InfoContainer, op.CodeGenerator, op.IsLazyMode));
                }
            }
        }

        private void OnSearchFilterChanged(ChangeEvent<string> evt)
        {
            RefreshUIBISDListView(BISDScannedData);
        }

        private void RefreshButtons()
        {
            if (runningBackgroundTasks.OfType<ExtractBISDInfoCommandThreaded>().Any())
            {
                RefreshBtn.SetEnabled(false);
            }
            else
            {
                RefreshBtn.SetEnabled(true);
            }
        }

        private void HandleTaskRemoved(ICommandBase cmd)
        {
            RefreshTasksText();
            RefreshButtons();
        }

        private void HandleTaskAdded(ICommandBase cmd)
        {
            RefreshTasksText();
            RefreshButtons();
        }

        private void RefreshTasksText()
        {
            sb.Clear();

            if (runningBackgroundTasks.Count == 0)
            {
                sb.Append("No backround tasks running ...");
            }
            else
            {
                sb.Append($"{runningBackgroundTasks.Count} task(s) running : ");
                for (int i = 0; i < runningBackgroundTasks.Count - 1; i++)
                {
                    ICommandBase t = runningBackgroundTasks[i];
                    sb.Append(' ');
                    sb.Append(t.GetType().Name);
                    sb.Append(',');
                }

                ICommandBase last = runningBackgroundTasks.Last();
                sb.Append(' ');
                sb.Append(last.GetType().Name);
                sb.Append('.');
            }

            LoadingTask.text = sb.ToString();

        }

        private void OnRefreshBtnClicked()
        {
            RefreshModels();
        }

        /// <summary>
        /// Retrieves selected folder on Project view.
        /// </summary>
        /// <returns></returns>
        public static string GetSelectedPathOrFallback()
        {
            string defaultPath = EditorUtils.CurrentProjectWindowPath();

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
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

            CommandManagerEditor.RunInstant(new CreateBehaviourFileCommand(modelName, relativePath));
            CommandManagerEditor.RunInstant(new CreateInstanceFileCommand(modelName, relativePath));
            CommandManagerEditor.RunInstant(new CreateInstancePartialFileCommand(modelName, relativePath));
            CommandManagerEditor.RunInstant(new CreateStateFileCommand(modelName, relativePath));
            CommandManagerEditor.RunInstant(new CreateDataFileCommand(modelName, relativePath));
            CommandManagerEditor.RunInstant(new CreateGameSaveFileCommand(modelName, relativePath));
            CommandManagerEditor.RunInstant(new CreateGameSaveHandlerFileCommand(modelName, relativePath));
        }

        private void OnModelNameChanged(ChangeEvent<string> evt)
        {
            RefreshGenerateModel();
        }

        private void RefreshGenerateModel()
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