using Bloodthirst.Core.BISD.CodeGeneration;
using Bloodthirst.Core.BISD.Editor.Commands;
using Bloodthirst.Editor;
using Bloodthirst.Editor.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.BISD.Editor
{
    public class BISDInfoUI
    {
        private const string UXML_INFO_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD/BISD Generator/EditorWindow/BISDInfoUI.uxml";
        public VisualElement VisualElement { get; private set; }

        private List<ICodeGenerator> codeGenerators;

        public string Model { get; }
        public BISDInfoContainer InfoContainer { get; }
        private Label ModelName => VisualElement.Q<Label>(nameof(ModelName));
        private Button ModelFolder => VisualElement.Q<Button>(nameof(ModelFolder));
        private ObjectField BehaviourAsset => VisualElement.Q<ObjectField>(nameof(BehaviourAsset));
        private VisualElement CodeGeneratorList => VisualElement.Q<VisualElement>(nameof(CodeGeneratorList));
        private ObjectField InstanceAsset => VisualElement.Q<ObjectField>(nameof(InstanceAsset));
        private ObjectField InstancePartialAsset => VisualElement.Q<ObjectField>(nameof(InstancePartialAsset));
        private ObjectField StateAsset => VisualElement.Q<ObjectField>(nameof(StateAsset));
        private ObjectField DataAsset => VisualElement.Q<ObjectField>(nameof(DataAsset));
        private ObjectField GameSaveAsset => VisualElement.Q<ObjectField>(nameof(GameSaveAsset));
        private ObjectField GameSaveHandlerAsset => VisualElement.Q<ObjectField>(nameof(GameSaveHandlerAsset));

        #region regenrate btns
        private Button BehaviourRegenerate => VisualElement.Q<Button>(nameof(BehaviourRegenerate));
        private Button InstanceRegenerate => VisualElement.Q<Button>(nameof(InstanceRegenerate));
        private Button InstancePartialRegenerate => VisualElement.Q<Button>(nameof(InstancePartialRegenerate));
        private Button StateRegenerate => VisualElement.Q<Button>(nameof(StateRegenerate));
        private Button DataRegenerate => VisualElement.Q<Button>(nameof(DataRegenerate));
        private Button GameDataRegenerate => VisualElement.Q<Button>(nameof(GameDataRegenerate));
        private Button LoadSaveHandlerRegenerate => VisualElement.Q<Button>(nameof(LoadSaveHandlerRegenerate));
        #endregion

        public List<CodeGenerationOptionUI> codeGenerationOptions = new List<CodeGenerationOptionUI>();

        public event Action<BISDInfoUI , CodeGenerationOptionUI> OnCodeGenerationChanged;

        public BISDInfoUI(string model , BISDInfoContainer infoContainer)
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_INFO_PATH);
            VisualElement template = visualTree.CloneTree();
            VisualElement = template.Children().First();

            // code generators
            codeGenerators = new List<ICodeGenerator>()
            {
                new ObservableFieldsCodeGenerator(),
                new GameSaveCodeGenerator(),
                new GameSaveHandlerCodeGenerator()
            };

            Model = model;
            InfoContainer = infoContainer;

            ModelName.text = model;
            ModelFolder.text = $"Folder : {infoContainer.ModelFolder}";

            ModelFolder.clickable.clicked -= HandleFolderBtn;
            ModelFolder.clickable.clicked += HandleFolderBtn;

            BehaviourAsset.objectType = typeof(TextAsset);
            InstanceAsset.objectType = typeof(TextAsset);
            InstancePartialAsset.objectType = typeof(TextAsset);
            StateAsset.objectType = typeof(TextAsset);
            DataAsset.objectType = typeof(TextAsset);
            GameSaveAsset.objectType = typeof(TextAsset);
            GameSaveHandlerAsset.objectType = typeof(TextAsset);

            BehaviourAsset.value = infoContainer?.Behaviour?.TextAsset;
            InstanceAsset.value = infoContainer?.InstanceMain?.TextAsset;
            InstancePartialAsset.value = infoContainer?.InstancePartial?.TextAsset;
            StateAsset.value = infoContainer?.State?.TextAsset;
            DataAsset.value = infoContainer?.Data?.TextAsset;
            GameSaveAsset.value = infoContainer?.GameSave?.TextAsset;
            GameSaveHandlerAsset.value = infoContainer?.GameSaveHandler?.TextAsset;

            BehaviourRegenerate.clickable.clicked += HandleGenerateBehaviour;
            InstanceRegenerate.clickable.clicked += HandleGenerateInstance;
            InstancePartialRegenerate.clickable.clicked += HandleGenerateInstancePartial;
            StateRegenerate.clickable.clicked += HandleGenerateState;
            DataRegenerate.clickable.clicked += HandleGenerateData;
            GameDataRegenerate.clickable.clicked += HandleGenerateGameData;
            LoadSaveHandlerRegenerate.clickable.clicked += HandleGenerateLoadSaveHandler;

            foreach(ICodeGenerator c in codeGenerators)
            {
                CodeGenerationOptionUI codeGenerationOption = new CodeGenerationOptionUI(c , this);

                codeGenerationOption.OnCodeGenerationChanged += HandleSubCodeGenerationChanged;

                CodeGeneratorList.Add(codeGenerationOption.VisualElement);

                codeGenerationOptions.Add(codeGenerationOption);
            }

            int labelMaxLenght = codeGenerators.Max(c => c.GetType().Name.Length) * 7;

            foreach(CodeGenerationOptionUI ui in codeGenerationOptions)
            {
                ui.SetLabelWidth(labelMaxLenght);
            }
            
        }

        private void HandleSubCodeGenerationChanged(BISDInfoUI infoUI, CodeGenerationOptionUI codeGenerationOptionUI )
        {
            OnCodeGenerationChanged?.Invoke(infoUI, codeGenerationOptionUI);
        }

        private void HandleClickGenerationBtn(EventBase eventBase)
        {
            Button senderBtn = (Button)eventBase.target;

            ICodeGenerator c = senderBtn.userData as ICodeGenerator;

            c.InjectGeneratedCode(InfoContainer);
        }

        private void HandleGenerateBehaviour()
        {
            CommandManagerEditor.RunInstant(new CreateBehaviourFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }

        private static void HandleAfterReload()
        {
            Debug.Log("Yoooo");
        }

        private void HandleGenerateInstance()
        {
            CommandManagerEditor.RunInstant(new CreateInstanceFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }

        private void HandleGenerateInstancePartial()
        {
            CommandManagerEditor.RunInstant(new CreateInstancePartialFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }


        private void HandleGenerateState()
        {
            CommandManagerEditor.RunInstant(new CreateStateFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }

        private void HandleGenerateData()
        {
            CommandManagerEditor.RunInstant(new CreateDataFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }

        private void HandleGenerateGameData()
        {
            CommandManagerEditor.RunInstant(new CreateGameSaveFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }

        private void HandleGenerateLoadSaveHandler()
        {
            CommandManagerEditor.RunInstant(new CreateGameSaveHandlerFileCommand(InfoContainer.ModelName, InfoContainer.ModelFolder));
        }

        private void HandleFolderBtn()
        {
            UnityEngine.Object folderAsset = AssetDatabase.LoadAssetAtPath(InfoContainer.ModelFolder , typeof(UnityEngine.Object));
            ProjectWindowUtil.ShowCreatedAsset(folderAsset);
        }
    }
}