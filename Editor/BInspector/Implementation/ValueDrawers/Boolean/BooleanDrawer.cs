using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class BooleanDrawer : ValueDrawerBase
    {
        private Toggle BoolField { get; set; }

        public override object DefaultValue()
        {
            return false;
        }

        public override void Tick()
        {
            if (BoolField.focusController.focusedElement == BoolField)
                return;

            BoolField.value = (bool)ValueProvider.Get();
        }

        protected override void GenerateDrawer(LayoutContext drawerContext)
        {
            BoolField = new Toggle();
            BoolField.SetValueWithoutNotify((bool)ValueProvider.Get());

            BoolField.AddToClassList("grow-1");            
            BoolField.RegisterValueChangedCallback(HandleValueChanged);

            DrawerContainer.AddToClassList("row");
            DrawerContainer.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });
            DrawerContainer.Add(BoolField);
        }
        
        private void HandleValueChanged(ChangeEvent<bool> evt)
        {
            DrawerValue = evt.newValue;
            ValueProvider.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }
        protected override void PostLayout() { }

        public override void Destroy()
        {
            BoolField.RegisterValueChangedCallback(HandleValueChanged);
            BoolField = null;
        }


    }
}