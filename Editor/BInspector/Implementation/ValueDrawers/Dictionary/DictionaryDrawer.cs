using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

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
        public IValueDrawer AddKeyDrawer { get; private set; }
        public IValueDrawer AddValueDrawer { get; private set; }
        public VisualElement AddKeyContainer { get; private set; }
        public VisualElement AddValueContainer { get; private set; }
        public Button AddElementBtn { get; private set; }
        public VisualElement ElementsContainer { get; private set; }

        private const string ScriptNameLabel = nameof(ScriptNameLabel);
        private const string ScriptIconImage = nameof(ScriptIconImage);
        private const string ScriptSelectionZone = nameof(ScriptSelectionZone);
        private const string Elements = nameof(Elements);
        private VisualElement UIContainer;

        public DictionaryDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            UIContainer = uxmlAsset.CloneTree();
            UIContainer.styleSheets.Add(ussAsset);
            UIContainer.AddToClassList("grow-1");
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

        protected override void Postsetup()
        {
            // get the dictionary types
            Type[] typeArgs = DrawerInfo.DrawerType().GenericTypeArguments;

            KeyType = typeArgs[0];
            ValueType = typeArgs[1];

            // start with the ui

            ElementsContainer = UIContainer.Q<VisualElement>(nameof(ElementsContainer));
            AddElementBtn = UIContainer.Q<Button>(nameof(AddElementBtn));
            AddKeyContainer = UIContainer.Q<VisualElement>(nameof(AddKeyContainer));
            AddValueContainer = UIContainer.Q<VisualElement>(nameof(AddValueContainer));

            // setup the add element section of the ui
            AddElementSection();

            // init uis

            if (Value == null)
                return;

            Refresh();
        }

        private void AddElementSection()
        {
            // setup add element

            AddKeyDrawer = ValueDrawerProvider.Get(KeyType);
            AddValueDrawer = ValueDrawerProvider.Get(ValueType);


            // key setup
            AddKeyDrawer.Setup(GetAddKeyDrawerInfo() , this, DrawerContext);

            // value setup
            AddValueDrawer.Setup(GetAddValueDrawerInfo(), this, DrawerContext);

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

        private void Refresh()
        {
            IDictionary dict = (IDictionary)Value;

            ElementsContainer.Clear();

            for (int i = 0; i < dict.Count; i++)
            {
                DictionaryElementDrawer elem = new DictionaryElementDrawer();
                elem.Setup(dict, i, KeyType, ValueType , DrawerContext);

                ElementsContainer.Add(elem.VisualElement);
            }
        }

        private void HandleAddElementClick(ClickEvent evt)
        {
            IDictionary dict = (IDictionary) Value;
            dict.Add(AddKeyDrawer.Value , AddValueDrawer.Value);
            Refresh();
        }

        public override void Clean()
        {
            AddElementBtn.UnregisterCallback<ClickEvent>(HandleAddElementClick);
        }
    }
}