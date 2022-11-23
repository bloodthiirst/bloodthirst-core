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
        private struct AddElementData
        {
            public IValueDrawer Key { get; set; }
            public IValueDrawer Value { get; set; }
            public IDictionary Dictionary { get; set; }
        }

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
        private LayoutContext cachedLayoutContext;

        public override void Tick()
        {
            Clean();
            Draw();
        }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            // header
            UIHeader = new VisualElement();
            UIHeader.AddToClassList("row");
            UIHeader.AddToClassList("grow-1");
            UIHeader.AddToClassList("shrink-1");
            UIHeader.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            // content
            UIContainer = new VisualElement();
            UIContainer.AddToClassList("column");
            UIContainer.AddToClassList("grow-1");

            DrawerContainer.AddToClassList("column");
            DrawerContainer.Add(UIHeader);
            DrawerContainer.Add(UIContainer);
        }

        public override object DefaultValue()
        {
            return null;
        }

        private void InstanceCreator()
        {
            Type type = ValueProvider.DrawerType();

            TypeSelector = new TypeSelector(type);
            TypeSelector.AddToClassList("grow-1");

            Type currentType = ValueProvider.Get() == null ? null : ValueProvider.Get().GetType();
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

            DrawerValue = evt.newValue;
            ValueProvider.Set(newInstance);

            TriggerOnValueChangedEvent();
            
            Clean();
            Draw();
        }

        protected override void PostLayout()
        {
            // get the dictionary types
            Type t = ValueProvider.DrawerType();
            Type[] typeArgs = t.GenericTypeArguments;

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

            AddKeyDrawer.DrawerValue = ReflectionUtils.GetDefaultValue(KeyType)();
            AddValueDrawer.DrawerValue = ReflectionUtils.GetDefaultValue(ValueType)();

            // key setup
            ValueDrawerUtils.DoLayout(AddKeyDrawer , null ,GetAddKeyDrawerInfo());
            
            // value setup
            ValueDrawerUtils.DoLayout(AddValueDrawer, null, GetAddValueDrawerInfo());

            AddKeyContainer.Add(AddKeyDrawer.DrawerContainer);
            AddValueContainer.Add(AddValueDrawer.DrawerContainer);

            // add btn
            AddElementBtn.RegisterCallback<ClickEvent>(HandleAddElementClick);
        }

        private ValueProviderGeneric GetAddKeyDrawerInfo()
        {
            return new ValueProviderGeneric(
                            // state
                            null,

                            // path
                            new ValuePath() { PathType = PathType.CUSTOM },

                            // getter
                            () =>
                            {
                                return AddKeyDrawer.DrawerValue;
                            },
                            // setter
                            (newKey) =>
                            {
                                AddKeyDrawer.DrawerValue = newKey;
                            },
                            // type
                            KeyType,

                            // index
                            null,
                            null
                            );
        }

        private ValueProviderGeneric GetAddValueDrawerInfo()
        {
            return new ValueProviderGeneric(
                            // state
                            null,

                            // path
                            new ValuePath() { PathType = PathType.CUSTOM },

                            // getter
                            () =>
                            {
                                return AddValueDrawer.DrawerValue;
                            },
                            // setter
                            (newValue) =>
                            {
                                AddValueDrawer.DrawerValue = newValue;

                            },
                            // type
                            ValueType,

                            // index
                            null,

                            // member info
                            null
                            );
        }

        private void Clean()
        {
            if(UIDictionary != null)
            {
                AddKeyDrawer.Destroy();
                AddValueDrawer.Destroy();

                AddKeyContainer.Remove(AddKeyDrawer.DrawerContainer);
                AddValueContainer.Remove(AddValueDrawer.DrawerContainer);

                AddElementBtn.UnregisterCallback<ClickEvent>(HandleAddElementClick);

                UIContainer.Remove(UIDictionary);

                AddKeyDrawer = null;
                AddValueDrawer = null;
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

            if (ValueProvider.Get() != null)
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


            IDictionary dict = (IDictionary)ValueProvider.Get();

            ElementsContainer.Clear();

            AddElementSection();

            for (int i = 0; i < dict.Count; i++)
            {
                DictionaryElementDrawer elem = new DictionaryElementDrawer();
                elem.Setup(dict, i, KeyType, ValueType, cachedLayoutContext);

                ElementsContainer.Add(elem.VisualElement);
            }
        }

        private void HandleAddElementClick(ClickEvent evt)
        {
            if(AddKeyDrawer.ValueProvider.Get() == null)
            {
                EditorUtility.DisplayDialog("Error", "Dictionary key can't be Null", "Ok");
                return;
            }
            
            IDictionary dict = (IDictionary)ValueProvider.Get();

            if (dict.Contains(AddKeyDrawer.ValueProvider.Get()))
            {
                EditorUtility.DisplayDialog("Error", "Key already exists", "Ok");
                return;
            }

            dict.Add(AddKeyDrawer.ValueProvider.Get(), AddValueDrawer.ValueProvider.Get());

            Clean();
            Draw();
        }

        public override void Destroy()
        {
            Clean();

         
            UIContainer.Clear();
            UIContainer = null;            
        }
    }
}