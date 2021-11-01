using Bloodthirst.Editor;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;
using ICodeGenerator = Bloodthirst.Core.BISD.CodeGeneration.ICodeGenerator;

namespace Bloodthirst.Core.BISD.Editor
{
    public class CodeGenerationOptionUI
    {
        private const string UXML_INFO_PATH = EditorConsts.GLOBAL_EDITOR_FOLRDER_PATH + "BISD Generator/EditorWindow/CodeGenerationOptionUI.uxml";
        private const string IS_UP_TO_DATE = "is-up-to-date";
        private const string IS_DIRTY = "is-dirty";
        public VisualElement VisualElement { get; private set; }
        private Label CodeGenerationName => VisualElement.Q<Label>(nameof(CodeGenerationName));
        private VisualElement IsDirty => VisualElement.Q<VisualElement>(nameof(IsDirty));
        private Toggle UseLazyMode => VisualElement.Q<Toggle>(nameof(UseLazyMode));
        private Toggle Selected => VisualElement.Q<Toggle>(nameof(Selected));
        public ICodeGenerator CodeGenerator { get; }
        public BISDInfoUI ParentUI { get; }
        public bool IsLazyMode {get; private set; }
        public bool IsSelected {get; private set; }

        public event Action<BISDInfoUI , CodeGenerationOptionUI> OnCodeGenerationChanged;

        public void SetLabelWidth(float width)
        {
            CodeGenerationName.style.width = new StyleLength(new Length(width , LengthUnit.Pixel));
        }

        public CodeGenerationOptionUI(ICodeGenerator codeGenerator, BISDInfoUI parentUI)
        {
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_INFO_PATH);
            VisualElement template = visualTree.CloneTree();
            VisualElement = template.Children().First();
            CodeGenerator = codeGenerator;
            ParentUI = parentUI;
            Selected.value = false;

            bool isDirty = codeGenerator.ShouldInject(ParentUI.InfoContainer);

            if(isDirty)
            {
                IsDirty.RemoveFromClassList(IS_UP_TO_DATE);
                IsDirty.AddToClassList(IS_DIRTY);
            }
            else
            {
                IsDirty.AddToClassList(IS_UP_TO_DATE);
                IsDirty.RemoveFromClassList(IS_DIRTY);
            }

            UseLazyMode.value = false;
            UseLazyMode.SetEnabled(false);

            CodeGenerationName.text = CodeGenerator.GetType().Name;
            UseLazyMode.RegisterValueChangedCallback(HandleUseLazyModeChanged);
            Selected.RegisterValueChangedCallback(HandleSelectedChanged);
        }

        private void HandleSelectedChanged(ChangeEvent<bool> evt)
        {
            IsSelected = evt.newValue;

            UseLazyMode.SetEnabled(IsSelected);

            OnCodeGenerationChanged?.Invoke( ParentUI, this);
        }

        private void HandleUseLazyModeChanged(ChangeEvent<bool> evt)
        {
            IsLazyMode = evt.newValue;
        }
    }
}