using Bloodthirst.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public struct IndexWrapper
    {
        public int Index { get; set; }
        public object Value { get; set; }
    }
    public class PopupWindowUI : EditorWindow
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Dropdown/PopupWindowUI/PopupWindowUI.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Dropdown/PopupWindowUI/PopupWindowUI.uss";

        public event Action<PopupWindowUI> OnClose;
        public event Action<IndexWrapper> OnValueChanged;
        private List<IndexWrapper> _AllValues { get; set; }
        private List<IndexWrapper> _FilteredValues { get; set; }
        public IReadOnlyList<IndexWrapper> FilteredValues => _FilteredValues;

        private VisualElement AttachedTo { get; set; }
        private EditorWindow Parent { get; set; }
        private PopupUI PopupUI { get; set; }


        private VisualElement root;
        private TextField SearchTxt => root.Q<TextField>(nameof(SearchTxt));
        private Button ClearBtn => root.Q<Button>(nameof(ClearBtn));
        private VisualElement ListContainer => root.Q<VisualElement>(nameof(ListContainer));
        private VisualElement ChoiceContainer => root.Q<VisualElement>(nameof(ChoiceContainer));

        private ListView ListUI { get; set; }

        private Func<object,string> _AsString;
        private Func<VisualElement> _MakeItem;
        private Action<VisualElement, int> _BindItem;

        private void OnEnable()
        {
          
        }

        private void RefrehsWindowSize()
        {
            Vector2 dropdownPos =
                // window offset
                Parent.position.position +

                // element offset
                AttachedTo.worldBound.center - (AttachedTo.worldBound.size * 0.5f) +

                // bottom left point of element
                new Vector2(0, AttachedTo.worldBound.height);

            Vector2 dropdownSize = new Vector2(AttachedTo.worldBound.width, ChoiceContainer.worldBound.height);

            root.style.height = dropdownSize.y;


            minSize = dropdownSize;
            maxSize = dropdownSize;
            position = new Rect(dropdownPos, dropdownSize);

            Focus();
        }

        private void OnDisable() { }

        private void CreateGUI()
        {
            root = rootVisualElement;

            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(root);
            
            root.style.flexGrow = 1;
            root.style.flexShrink = 1;


            root.styleSheets.Add(styleSheet);
            root.styleSheets.Add(EditorConsts.GlobalStyleSheet);

            InitializeUI();
        }

        private void InitializeUI()
        {
            ClearBtn.clickable.clicked -= HandleClearBtnClicked;
            ClearBtn.clickable.clicked += HandleClearBtnClicked;

            SearchTxt.UnregisterValueChangedCallback(HandleSearchTxtChanged);
            SearchTxt.RegisterValueChangedCallback(HandleSearchTxtChanged);

            SearchTxt.Focus();
        }

        private void HandleAttatchedToChanged(GeometryChangedEvent evt)
        {
            RefrehsWindowSize();
        }

        private void HandleSizeChanged(GeometryChangedEvent evt)
        {
            RefrehsWindowSize();
        }

        private void HandleSearchTxtChanged(ChangeEvent<string> evt)
        {
            _FilteredValues.Clear();

            if (string.IsNullOrWhiteSpace(evt.newValue))
            {
                for (int i = 0; i < _AllValues.Count; i++)
                {
                    IndexWrapper v = _AllValues[i];
                    _FilteredValues.Add(v);
                }
            }
            else
            {
                string searchTerm = evt.newValue.ToLowerInvariant();

                for (int i = 0; i < _AllValues.Count; i++)
                {
                    IndexWrapper curItem = _AllValues[i];

                    string currItemAsString = _AsString(curItem);

                    if (currItemAsString.Contains(searchTerm))
                    {
                        _FilteredValues.Add(curItem);
                    }
                }
            }

            ListUI.RefreshItems();
        }

        private void HandleClearBtnClicked()
        {
            SearchTxt.value = string.Empty;
        }

        public void Prepare(PopupUI popup, EditorWindow _Parent, VisualElement attachedTo)
        {
            PopupUI = popup;
            AttachedTo = attachedTo;
            Parent = _Parent;

            RefrehsWindowSize();
        }

        

        public void Setup(IList<IndexWrapper> source, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem , Func<object,string> asString)
        {
            _FilteredValues = new List<IndexWrapper>(source);
            _AllValues = new List<IndexWrapper>(source);

            _AsString = asString;
            _MakeItem = makeItem;
            _BindItem = bindItem;

            ListUI = new ListView(_FilteredValues, makeItem: makeItem, bindItem: bindItem);
            ListUI.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            ListUI.horizontalScrollingEnabled = false;

            ListUI.onSelectionChange -= HandleItemChosen;
            ListUI.onSelectionChange += HandleItemChosen;

            ListContainer.UnregisterCallback<GeometryChangedEvent>(HandleSizeChanged);
            ListContainer.RegisterCallback<GeometryChangedEvent>(HandleSizeChanged);

            AttachedTo.UnregisterCallback<GeometryChangedEvent>(HandleAttatchedToChanged);
            AttachedTo.RegisterCallback<GeometryChangedEvent>(HandleAttatchedToChanged);

            ListContainer.Add(ListUI);
        }

        private void HandleItemChosen(IEnumerable<object> obj)
        {
            OnValueChanged?.Invoke((IndexWrapper) obj.First());
            Close();
        }

        private void OnDestroy()
        {
            OnClose?.Invoke(this);
            OnClose = null;
        }

    }
}