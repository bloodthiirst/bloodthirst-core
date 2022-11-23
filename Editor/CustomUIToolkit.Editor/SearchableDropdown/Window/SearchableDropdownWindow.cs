using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class SearchableDropdownWindow : EditorWindow
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomUIToolkit.Editor/SearchableDropdown/Window/SearchableDropdownWindow.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomUIToolkit.Editor/SearchableDropdown/Window/SearchableDropdownWindow.uss";

        public event Action<SearchableDropdownWindow> OnClose;
        public event Action<IndexWrapper> OnValueChanged;
        
        private VisualElement AttachedTo { get; set; }
        private EditorWindow Parent { get; set; }
        private SearchableDropdown PopupUI { get; set; }

        private VisualElement root;
        public SearchableList SearchableList => root.Q<SearchableList>(nameof(SearchableList));
        private VisualElement ChoiceContainer => root.Q<VisualElement>(nameof(ChoiceContainer));

        private Func<IndexWrapper, string, bool> _FilterCondition;
        private Func<VisualElement> _MakeItem;
        private Action<VisualElement, int> _BindItem;

        private void OnEnable()
        {
          
        }
        public void PlaceDropdown()
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
        }

        private void OnLostFocus()
        {
            Close();
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
            if (!root.styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }

            InitializeUI();
            ListenUI();
        }


        private void InitializeUI()
        {
            SearchableList.Focus();

        }

        private void ListenUI()
        {
            ChoiceContainer.RegisterCallback<GeometryChangedEvent>(HandleWindowChanged);
        }

        public void Prepare(SearchableDropdown popup, EditorWindow _Parent, VisualElement attachedTo)
        {
            PopupUI = popup;
            AttachedTo = attachedTo;
            Parent = _Parent;
        }

        public void Setup(IList source, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem, Func<IndexWrapper, string, bool> filterCondition)
        {
            _FilterCondition = filterCondition;
            _MakeItem = makeItem;
            _BindItem = bindItem;

            SearchableList.Setup(source, filterCondition, makeItem, bindItem, HandleItemChosen);
        }

        private void HandleWindowChanged(GeometryChangedEvent evt)
        {
            PlaceDropdown();
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