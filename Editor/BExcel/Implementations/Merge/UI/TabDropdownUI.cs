using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public class TabDropdownUI : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Implementations/Merge/UI/TabDropdownUI.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Implementations/Merge/UI/TabDropdownUI.uss";

        public event Action<string> OnValueChanged;

        public event Action<TabDropdownUI> OnRemoveTriggered;
        private VisualElement TabContainer => this.Q<VisualElement>(nameof(TabContainer));
        private PopupField<string> TabDropdown { get; set; }
        private Button RemoveBtn => this.Q<Button>(nameof(RemoveBtn));


        public string Value => TabDropdown.value;

        private string Format(string s)
        {
            return s;
        }

        public TabDropdownUI(List<string> values)
        {
            // UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            // USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            styleSheets.Add(styleSheet);

            // btn
            RemoveBtn.text = "Remove";
            RemoveBtn.clickable.clicked += HandleRemoveClicked;
            
            // drop                       down
            TabDropdown = new PopupField<string>(values, 0, Format, Format);
            TabDropdown.RegisterValueChangedCallback(HandleValueChanged);


            TabContainer.Add(TabDropdown);
        }

        public void UpdateSource(List<string> source)
        {
            string oldValue = Value;

            bool hasValue = false;

            if(source.Contains(Value))
            {
                hasValue = true;
            }

            TabContainer.Remove(TabDropdown);
            TabDropdown = new PopupField<string>(source, 0, Format, Format);
            TabDropdown.RegisterValueChangedCallback(HandleValueChanged);


            TabContainer.Add(TabDropdown);

            if (hasValue)
            {
                TabDropdown.value = oldValue;
                return;
            }

            OnValueChanged?.Invoke(TabDropdown.value);
        }


        private void HandleRemoveClicked()
        {
            OnRemoveTriggered?.Invoke(this);
        }

        private void HandleValueChanged(ChangeEvent<string> evt)
        {
            OnValueChanged?.Invoke(evt.newValue);
        }
    }
}