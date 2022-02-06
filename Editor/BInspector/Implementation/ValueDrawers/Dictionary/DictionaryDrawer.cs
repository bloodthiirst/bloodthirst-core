using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using Bloodthirst.Core.Utils;

namespace Bloodthirst.Editor.BInspector
{
    public class DictionaryDrawer : ValueDrawerBase
    {
        private const string DICT_PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/Dictionary/DictionaryDrawer.uxml";
        private const string ELEMENT_PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/Dictionary/Element/DictionaryElementDrawer.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/Dictionary/DictionaryDrawer.uss";

        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DICT_PATH_UXML);
        private static VisualTreeAsset uxmlElementAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ELEMENT_PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);
        public Type KeyType { get; private set; }
        public Type ValueType { get; private set; }

        public Button AddElementBtn { get; private set; }

        private TypeSelector TypeSelector { get; set; }

        private const string ScriptNameLabel = nameof(ScriptNameLabel);
        private const string ScriptIconImage = nameof(ScriptIconImage);
        private const string ScriptSelectionZone = nameof(ScriptSelectionZone);
        private const string Elements = nameof(Elements);
        private VisualElement UIHeader { get; set; }
        private VisualElement UIContainer { get; set; }
        private VisualElement UIDictionary { get;  set; }
        public VisualElement AddKeyContainer { get; private set; }
        public VisualElement AddValueContainer { get; private set; }
        public IValueDrawer AddKeyDrawer { get; private set; }
        public IValueDrawer AddValueDrawer { get; private set; }
        public VisualElement ElementsContainer { get; private set; }

        public DictionaryDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("column");

            UIHeader = new VisualElement();
            UIHeader.AddToClassList("row");
            UIHeader.AddToClassList("grow-1");

            UIHeader.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            // load dictionary ui
            UIContainer = new VisualElement();
            UIContainer.AddToClassList("column");
            UIContainer.AddToClassList("grow-1");

            root.Add(UIHeader);
            root.Add(UIContainer);
        }

        public override object DefaultValue()
        {
            return null;
        }

        private struct AddElementData
        {
            public IValueDrawer Key { get; set; }
            public IValueDrawer Value { get; set; }
            public IDictionary Dictionary { get; set; }
        }

        private void InstanceCreator()
        {
            Type type = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo);

            TypeSelector = new TypeSelector(type);
            TypeSelector.AddToClassList("grow-1");

            Type currentType = Value == null ? null : Value.GetType();
            TypeSelector.SetValueWithoutNotify(currentType);

            TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
            TypeSelector.RegisterValueChangedCallback(HandleValueTypeChanged);

            UIHeader.Add(TypeSelector);
        }

        private void HandleValueTypeChanged(ChangeEvent<Type> evt)
        {
            Type newType = evt.newValue;

            object newInstance = null;

            if (newType != null)
            {
                newInstance = Activator.CreateInstance(newType);
            }

            DrawerInfo.Set(newInstance);

            TriggerOnValueChangedEvent();
            
            Clean();
            Draw();
        }

        protected override void Postsetup()
        {
            // get the dictionary types
            Type[] typeArgs = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo).GenericTypeArguments;

            KeyType = typeArgs[0];
            ValueType = typeArgs[1];

            // init uis
            Draw();
        }

        private void AddElementSection()
        {
            // setup add element
            AddKeyDrawer = ValueDrawerProvider.Get(KeyType);
            AddValueDrawer = ValueDrawerProvider.Get(ValueType);


            DrawerContext cpy = DrawerContext;
            cpy.IndentationLevel = 0;

            // key setup
            AddKeyDrawer.Setup(GetAddKeyDrawerInfo() , this, cpy);

            // value setup
            AddValueDrawer.Setup(GetAddValueDrawerInfo(), this, cpy);

            AddKeyContainer.Add(AddKeyDrawer.DrawerRoot);
            AddValueContainer.Add(AddValueDrawer.DrawerRoot);

            // add btn
            AddElementBtn.RegisterCallback<ClickEvent>(HandleAddElementClick);
        }

        private ValueDrawerInfoGeneric GetAddKeyDrawerInfo()
        {
            return new ValueDrawerInfoGeneric(
                            // state
                            null,

                            // getter
                            () =>
                            {
                                return AddKeyDrawer.Value;
                            },
                            // setter
                            (newKey) =>
                            {
                                AddKeyDrawer.Value = newKey;

                            },
                            // type
                            KeyType,

                            // index
                            null
                            );
        }

        private ValueDrawerInfoGeneric GetAddValueDrawerInfo()
        {
            return new ValueDrawerInfoGeneric(
                            // state
                            null,

                            // getter
                            () =>
                            {
                                return AddValueDrawer.Value;
                            },
                            // setter
                            (newValue) =>
                            {
                                AddValueDrawer.Value = newValue;

                            },
                            // type
                            ValueType,

                            // index
                            null
                            );
        }

        private void Clean()
        {
            if(UIDictionary != null)
            {
                AddElementBtn.UnregisterCallback<ClickEvent>(HandleAddElementClick);

                UIContainer.Remove(UIDictionary);
                
                AddKeyContainer = null;
                AddValueContainer = null;
                AddElementBtn = null;
                ElementsContainer = null;
                UIDictionary = null;
            }

            if (TypeSelector != null)
            {
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                UIHeader.Remove(TypeSelector);
                TypeSelector = null;
            }
        }

        private void Draw()
        {
            InstanceCreator();

            if (Value != null)
            {
                HasValue();
            }
        }

        private void HasValue()
        {
            UIDictionary = uxmlAsset.CloneTree();
            UIDictionary.styleSheets.Add(ussAsset);

            UIContainer.Add(UIDictionary);

            AddKeyContainer = UIDictionary.Q<VisualElement>(nameof(AddKeyContainer));
            AddValueContainer = UIDictionary.Q<VisualElement>(nameof(AddValueContainer));
            AddElementBtn = UIDictionary.Q<Button>(nameof(AddElementBtn));

            ElementsContainer = UIDictionary.Q<VisualElement>(nameof(ElementsContainer));


            IDictionary dict = (IDictionary)Value;

            ElementsContainer.Clear();

            AddElementSection();

            for (int i = 0; i < dict.Count; i++)
            {
                DictionaryElementDrawer elem = new DictionaryElementDrawer();
                elem.Setup(dict, i, KeyType, ValueType, DrawerContext);

                ElementsContainer.Add(elem.VisualElement);
            }
        }

        private void HandleAddElementClick(ClickEvent evt)
        {
            if(AddKeyDrawer.Value == null)
            {
                EditorUtility.DisplayDialog("Error", "Dictionary key can't be Null", "Ok");
                return;
            }
            
            IDictionary dict = (IDictionary)Value;

            if (dict.Contains(AddKeyDrawer.Value))
            {
                EditorUtility.DisplayDialog("Error", "Key already exists", "Ok");
                return;
            }

            dict.Add(AddKeyDrawer.Value , AddValueDrawer.Value);

            Clean();
            Draw();
        }

        public override void Destroy()
        {
            if(TypeSelector != null)
            {
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                UIHeader.Remove(TypeSelector);
                TypeSelector = null;
            }

            UIContainer.Clear();
            UIContainer = null;

            AddElementBtn.UnregisterCallback<ClickEvent>(HandleAddElementClick);
            
        }
    }
}