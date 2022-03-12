using Bloodthirst.Core.Utils;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class UnityObjectBindableUI : IBindableUI
    {
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Bindable/Types/UnityObject/UnityObjectBindableUI.uss";

        private const string UXML_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Bindable/Types/UnityObject/UnityObjectBindableUI.uxml";
      

        private static StyleSheet styleSheet;
        private static StyleSheet StyleSheet
        {
            get
            {
                if(styleSheet == null)
                {
                    
                    styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
                }

                return styleSheet;
            }
        }


        private static VisualTreeAsset template;
        private static VisualTreeAsset Template
        {
            get
            {
                if (template == null)
                {

                    template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
                }

                return template;
            }
        }
        public VisualElement VisualElement { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public ObjectField TextUI => VisualElement.Q<ObjectField>(nameof(TextUI));

        private object instance;
        private Action<object, object> Setter { get; set; }
        private Func<object, object> Getter { get; set; }


        public void Setup(object instance, MemberInfo member)
        {
            this.instance = instance;
            this.MemberInfo = member;

            // create ui
            VisualElement = Template.CloneTree();
            VisualElement.styleSheets.Add(StyleSheet);

            TextUI.label = member.Name;
            TextUI.objectType = ReflectionUtils.GetMemberType(member);

            // generate getter/setter
            // if property
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo casted = (PropertyInfo)member;

                if(casted.CanRead)
                {
                    Getter = ReflectionUtils.EmitPropertyGetter(casted);
                }

                if (casted.CanWrite)
                {
                    Setter = ReflectionUtils.EmitPropertySetter(casted);
                }
                else
                {
                    TextUI.SetEnabled(false);
                }
            }

            // if field
            else
            {
                FieldInfo casted = (FieldInfo)member;
                Setter = ReflectionUtils.EmitFieldSetter(casted);
                Getter = ReflectionUtils.EmitFieldGetter(casted);
            }

            TextUI.RegisterValueChangedCallback(OnTextChanged);
            
            if (Getter != null)
            {
                TextUI.SetValueWithoutNotify((UnityEngine.Object)Getter(instance));
            }

        }

        private void OnTextChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            Setter?.Invoke(instance, evt.newValue);
        }

        public void CleanUp()
        {
            TextUI.UnregisterValueChangedCallback(OnTextChanged);
        }
    }
}
