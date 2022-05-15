
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class PopupUI : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Dropdown/PopupUI/PopupUI.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Dropdown/PopupUI/PopupUI.uss";

        private IndexWrapper _currentValue;
        public IndexWrapper CurrentValue 
        {
            get => _currentValue;
            private set
            {
                if (_currentValue.Index == value.Index)
                    return;

                _currentValue = value;
                CurrentValueTxt.text = _SelectedAsString(_currentValue);
                OnValueChanged?.Invoke(_currentValue);
            }
        }

        public event Action<IndexWrapper> OnValueChanged;
        private List<IndexWrapper> _Values { get; set; }
        public IReadOnlyList<IndexWrapper> AllValues => _Values;

        private EditorWindow _Parent { get; }
        private VisualElement SelectorBtn => this.Q<VisualElement>(nameof(SelectorBtn));
        private Label FieldName => this.Q<Label>(nameof(FieldName));
        private Label CurrentValueTxt => this.Q<Label>(nameof(CurrentValueTxt));
        public PopupWindowUI CurrentDropdown { get; private set; }

        private Func<VisualElement> _MakeItem;
        private Action<VisualElement, int> _BindItem;
        private Func<object, string> _SearchTerm;
        private Func<object, string> _SelectedAsString;

        private VisualElement DefaultMakeItem()
        {
            DefaultPopupUI ui = new DefaultPopupUI();
            return ui;
        }

        private string DefaultSelectedAsString(object value)
        {
            IndexWrapper casted = (IndexWrapper)value;
            string str = casted.Value.ToString();
            str = str.ToLowerInvariant();
            return str;
        }

        private string DefaultSearchTerm(object value)
        {
            IndexWrapper casted = (IndexWrapper)value;
            string str = casted.Value.ToString();
            str = str.ToLowerInvariant();
            return str;
        }
        private void DefaultBindItem(VisualElement ui , int index)
        {
            DefaultPopupUI casted = (DefaultPopupUI)ui;

            IndexWrapper filtered_wrapper = CurrentDropdown.FilteredValues[index];

            IndexWrapper actual_wrapper = _Values[filtered_wrapper.Index];

            casted.Setup(actual_wrapper);
        }

        public PopupUI(string label , int index, IList source, EditorWindow parent)
        {
            _Parent = parent;
            _MakeItem = DefaultMakeItem;
            _BindItem = DefaultBindItem;
            _SearchTerm = DefaultSearchTerm;
            _SelectedAsString = DefaultSelectedAsString;


            _Values = new List<IndexWrapper>();

            for (int i = 0; i < source.Count; i++)
            {
                object curr = source[i];

                IndexWrapper wrapped = new IndexWrapper() { Index = i, Value = curr };

                _Values.Add(wrapped);
            }


            SetupElement();

            CurrentValue = AllValues[index];
            FieldName.text = label;
        }
        public PopupUI(string label, int index, IList source, EditorWindow parent, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem, Func<object, string> searchTerm , Func<object , string> selectedAsString)
        {
            _Parent = parent;
            _MakeItem = makeItem;
            _BindItem = bindItem;
            _SearchTerm = searchTerm;
            _SelectedAsString = selectedAsString;

            _Values = new List<IndexWrapper>();

            for (int i = 0; i < source.Count; i++)
            {
                object curr = source[i];

                IndexWrapper wrapped = new IndexWrapper() { Index = i, Value = curr };

                _Values.Add(wrapped);
            }

            SetupElement();

            CurrentValue = AllValues[index];
            FieldName.text = label;
        }

        private void SetupElement()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            styleSheets.Add(styleSheet);
            styleSheets.Add(EditorConsts.GlobalStyleSheet);

            InitializeUI();
        }

        private void InitializeUI()
        {
            SelectorBtn.UnregisterCallback<BlurEvent>(HandleBlurEvent);
            SelectorBtn.RegisterCallback<BlurEvent>(HandleBlurEvent);

            SelectorBtn.UnregisterCallback<DetachFromPanelEvent>(HandleOnDestroy);
            SelectorBtn.RegisterCallback<DetachFromPanelEvent>(HandleOnDestroy);

            SelectorBtn.UnregisterCallback<ClickEvent>(HandleSelectorBtnClicked);
            SelectorBtn.RegisterCallback<ClickEvent>(HandleSelectorBtnClicked);
        }

        private void HandleBlurEvent(BlurEvent evt)
        {
            if (CurrentDropdown == null)
                return;

            if (!CurrentDropdown.hasFocus)
            {
                CurrentDropdown.Close();
                return;
            }

            if (evt.relatedTarget == null && !CurrentDropdown.hasFocus)
            {
                CurrentDropdown.Close();
            }
            else if (evt.relatedTarget != null )
            {
                CurrentDropdown.Close();
            }
        }

        private void HandleOnDestroy(DetachFromPanelEvent evt)
        {
            if (CurrentDropdown == null)
                return;

            if (evt.destinationPanel != null)
                return;

            CurrentDropdown.Close();
        }

        private void HandleSelectorBtnClicked(ClickEvent evt)
        {
            if (CurrentDropdown == null)
            {
                PopupWindowUI window = EditorWindow.CreateInstance<PopupWindowUI>();

                window.OnClose -= HandleDropdownClosed;
                window.OnClose += HandleDropdownClosed;

                window.OnValueChanged -= HandleValueChanged;
                window.OnValueChanged += HandleValueChanged;

                window.ShowPopup();
                window.Prepare(this, _Parent, SelectorBtn);
                window.Setup(_Values, _MakeItem, _BindItem , _SearchTerm);

                CurrentDropdown = window;
            }
            else
            {
                CurrentDropdown.Close();
            }
        }

        private void HandleValueChanged(IndexWrapper obj)
        {
            CurrentValue = obj;
        }

        private void HandleDropdownClosed(PopupWindowUI obj)
        {
            if (CurrentDropdown == null)
                return;

            CurrentDropdown.OnClose -= HandleDropdownClosed;
            CurrentDropdown = null;
        }
    }
}
