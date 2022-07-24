
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class SearchableDropdown : VisualElement
    {        
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/SearchableDropdown/Element/SearchableDropdown.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/SearchableDropdown/Element/SearchableDropdown.uss";

        public static readonly IndexWrapper EmptyEntry = new IndexWrapper() { Index = -1, Value = null };

        private IndexWrapper _currentValue;
        public IndexWrapper CurrentValue
        {
            get => _currentValue;
            private set
            {
                if (_currentValue.Index == value.Index)
                    return;

                CurrentValueForce = value;
            }
        }

        public IndexWrapper CurrentValueForce
        {
            set
            {
                _currentValue = value;

                if (_currentValue.Index == EmptyEntry.Index)
                {
                    CurrentValueTxt.text = "No Value Selected ...";
                }
                else
                {
                    CurrentValueTxt.text = _SelectedAsString(_currentValue);
                }

                OnValueChanged?.Invoke(_currentValue);
            }
        }

        public event Action<IndexWrapper> OnValueChanged;
        private List<object> InternalValues { get; set; }
        private List<IndexWrapper> _Values { get; set; }
        public IReadOnlyList<IndexWrapper> AllValues => _Values;

        private EditorWindow _Parent { get; }
        private VisualElement SelectorBtn => this.Q<VisualElement>(nameof(SelectorBtn));
        private Label FieldName => this.Q<Label>(nameof(FieldName));
        private Label CurrentValueTxt => this.Q<Label>(nameof(CurrentValueTxt));
        public SearchableDropdownWindow CurrentDropdown { get; private set; }

        private Func<VisualElement> _MakeItem;
        private Action<VisualElement, int> _BindItem;
        private Func<IndexWrapper , string , bool> _FilterCondition;
        private Func<object, string> _SelectedAsString;

        private VisualElement DefaultMakeItem()
        {
            SearchableDropdownDefaultElement ui = new SearchableDropdownDefaultElement();
            return ui;
        }
        private string DefaultSelectedAsString(object value)
        {
            IndexWrapper casted = (IndexWrapper)value;
            string str = casted.Value.ToString();
            str = str.ToLowerInvariant();
            return str;
        }
        private string DefaultSearchTerm(IndexWrapper value)
        {
            string str = value.Value.ToString();
            str = str.ToLowerInvariant();
            return str;
        }
        private void DefaultBindItem(VisualElement ui , int index)
        {
            SearchableDropdownDefaultElement casted = (SearchableDropdownDefaultElement)ui;

            IndexWrapper filtered_wrapper = CurrentDropdown.SearchableList.CurrentValues[index];

            IndexWrapper actual_wrapper = _Values[filtered_wrapper.Index];

            casted.Setup(actual_wrapper);
        }

        public void UpdateSource(IList newSource)
        {
            _Values.Clear();

            for (int i = 0; i < newSource.Count; i++)
            {
                object curr = newSource[i];

                IndexWrapper wrapped = new IndexWrapper() { Index = i, Value = curr };

                _Values.Add(wrapped);
            }
        }

        public void SetCurrentValueWithoutNotify(int index)
        {
            _currentValue = _Values[index];
            CurrentValueTxt.text = _SelectedAsString(_currentValue);
        }

        public void SetCurrentValue(int index)
        {
            CurrentValue = AllValues[index];
        }

        public SearchableDropdown(string label, int index, IList source, EditorWindow parent, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem, Func<IndexWrapper, string, bool> searchTerm , Func<object , string> selectedAsString)
        {
            _Parent = parent;
            _MakeItem = makeItem;
            _BindItem = bindItem;
            _FilterCondition = searchTerm;
            _SelectedAsString = selectedAsString;

            _Values = new List<IndexWrapper>();
            InternalValues = new List<object>();

            for (int i = 0; i < source.Count; i++)
            {
                object curr = source[i];

                IndexWrapper wrapped = new IndexWrapper() { Index = i, Value = curr };

                _Values.Add(wrapped);
                InternalValues.Add(curr);
            }

            SetupElement();
            FieldName.text = label;

            if (!Utils.MathUtils.IsBetween(index, 0, source.Count - 1))
            {
                CurrentValueForce = EmptyEntry;
            }
            else
            {
                CurrentValueForce = AllValues[index];
            }

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
                SearchableDropdownWindow window = EditorWindow.CreateInstance<SearchableDropdownWindow>();

                window.OnClose -= HandleDropdownClosed;
                window.OnClose += HandleDropdownClosed;

                window.OnValueChanged -= HandleValueChanged;
                window.OnValueChanged += HandleValueChanged;

                window.ShowPopup();
                window.Prepare(this, _Parent, SelectorBtn);
                window.Setup(InternalValues, _MakeItem, _BindItem , _FilterCondition);

               

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

        private void HandleDropdownClosed(SearchableDropdownWindow obj)
        {
            if (CurrentDropdown == null)
                return;

            CurrentDropdown.OnClose -= HandleDropdownClosed;
            CurrentDropdown = null;
        }
    }
}
