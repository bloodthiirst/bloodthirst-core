using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class FloatDrawer : ValueDrawerBase
    {
        private FloatField FloatField { get; set; }

        public FloatDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            FloatField = new FloatField();
            FloatField.AddToClassList("grow-1");
            root.Add(FloatField);
        }

        public override object DefaultValue()
        {
            return 0;
        }

        protected override void Postsetup()
        {
            FloatField.SetValueWithoutNotify((float)Value);
            FloatField.RegisterValueChangedCallback(HandleValueChanged);
        }

        private void HandleValueChanged(ChangeEvent<float> evt)
        {
            DrawerInfo.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public  override void Destroy()
        {
            FloatField.RegisterValueChangedCallback(HandleValueChanged);
            FloatField = null;
        }
    }
}