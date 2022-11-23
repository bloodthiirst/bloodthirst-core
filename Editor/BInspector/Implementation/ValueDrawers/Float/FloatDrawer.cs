using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class FloatDrawer : ValueDrawerBase
    {
        private FloatField FloatField { get; set; }

        public override object DefaultValue()
        {
            return 0;
        }

        public override void Tick()
        {
            if (FloatField.focusController == null)
                return;

            if (FloatField.focusController.focusedElement == FloatField)
                return;

            FloatField.value = (float)ValueProvider.Get();
        }


        protected override void GenerateDrawer(LayoutContext drawerContext)
        {
            FloatField = new FloatField();
            FloatField.AddToClassList("grow-1");
            FloatField.AddToClassList("shrink-1");
            FloatField.SetValueWithoutNotify((float)ValueProvider.Get());
            FloatField.RegisterValueChangedCallback(HandleValueChanged);

            DrawerContainer.AddToClassList("row");
            DrawerContainer.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });
            DrawerContainer.Add(FloatField);
        }

        private void HandleValueChanged(ChangeEvent<float> evt)
        {
            DrawerValue = evt.newValue;
            ValueProvider.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        protected override void PostLayout() { }

        public override void Destroy()
        {
            FloatField.RegisterValueChangedCallback(HandleValueChanged);
            FloatField = null;
        }
    }
}