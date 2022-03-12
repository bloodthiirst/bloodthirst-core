using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class EnumBindableUI : IBindableUI
    {
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Bindable/Types/Enum/EnumBindableUI.uss";

        private const string UXML_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Bindable/Types/Enum/EnumBindableUI.uxml";
      

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
        public VisualElement PopUpContainer => VisualElement.Q<VisualElement>(nameof(PopUpContainer));
        public PopupField<int> EnumPopUp { get; set; }

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

            Type t = ReflectionUtils.GetMemberType(member);

            int[] vals = Enum.GetValues(t) as int[];
            string[] names = Enum.GetNames(t) as string[];

            EnumPopUp = new PopupField<int>(member.Name, vals.ToList() , vals[0], (i) => names[i] , (i) => names[i]);
            EnumPopUp.AddToClassList("m-0");
            PopUpContainer.Add(EnumPopUp);


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
                    EnumPopUp.SetEnabled(false);
                }
            }

            // if field
            else
            {
                FieldInfo casted = (FieldInfo)member;
                Setter = ReflectionUtils.EmitFieldSetter(casted);
                Getter = ReflectionUtils.EmitFieldGetter(casted);
            }

            EnumPopUp.RegisterValueChangedCallback(OnValueChanged);
            
            if (Getter != null)
            {
                EnumPopUp.SetValueWithoutNotify((int)Getter(instance));
            }

        }

        private void OnValueChanged(ChangeEvent<int> evt)
        {
            Setter?.Invoke(instance, evt.newValue);
        }

        public void CleanUp()
        {
            EnumPopUp.UnregisterValueChangedCallback(OnValueChanged);
        }
    }
}
