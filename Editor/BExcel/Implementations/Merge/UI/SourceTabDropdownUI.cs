using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BExcelEditor
{
    public class SourceTabDropdownUI : VisualElement
    {
        public event Action<string> OnValueChanged;
        private PopupField<string> FilterDropdown { get; set; }

        public string Value => FilterDropdown.value;

        private string Format(string s)
        {
            return s;
        }

        public SourceTabDropdownUI(List<string> values)
        {
            FilterDropdown = new PopupField<string>(values, 0, Format, Format);
            FilterDropdown.RegisterValueChangedCallback(HandleValueChanged);

            Add(FilterDropdown);
        }

        private void HandleValueChanged( ChangeEvent<string> evt)
        {
            OnValueChanged?.Invoke(evt.newValue);
        }
    }
}