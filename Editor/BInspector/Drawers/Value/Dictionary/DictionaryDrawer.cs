using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace Bloodthirst.Editor.BInspector
{
    public class DictionaryDrawer : IValueDrawer
    {
        private const string DICT_PATH_UXML =       "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Dictionary/DictionaryDrawer.uxml";
        private const string ELEMENT_PATH_UXML =    "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Dictionary/DictionaryElementDrawer.uxml";
        private const string PATH_USS =             "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Dictionary/DictionaryDrawer.uss";

        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(DICT_PATH_UXML);
        private static VisualTreeAsset uxmlElementAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ELEMENT_PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);

        public object Value { get; set; }

        public VisualElement VisualElement { get; private set; }
        public IDrawerInfo DrawerInfo { get; private set; }
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

        void IValueDrawer.Initialize()
        {

        }
        public object DefaultValue()
        {
            return null;
        }

        public DictionaryDrawer()
        {
            VisualElement ui = uxmlAsset.CloneTree();
            ui.styleSheets.Add(ussAsset);
            VisualElement = ui;
        }

        private struct AddElementData
        {
            public IValueDrawer Key { get; set; }
            public IValueDrawer Value { get; set; }
            public IDictionary Dictionary { get; set; }
        }

        void IValueDrawer.Setup(IDrawerInfo drawerInfo)
        {
            // init values & data
            DrawerInfo = drawerInfo;
            Value = drawerInfo.Get();

            // get the dictionary types
            Type[] typeArgs = drawerInfo.DrawerType().GenericTypeArguments;

            KeyType = typeArgs[0];
            ValueType = typeArgs[1];

            // start with the ui

            ElementsContainer = VisualElement.Q<VisualElement>(nameof(ElementsContainer));
            AddElementBtn = VisualElement.Q<Button>(nameof(AddElementBtn));
            AddKeyContainer = VisualElement.Q<VisualElement>(nameof(AddKeyContainer));
            AddValueContainer = VisualElement.Q<VisualElement>(nameof(AddValueContainer));

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
            AddKeyDrawer.Setup(new DrawerInfoGeneric(
                // index
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
                KeyType
                )
            );

            // value setup
            AddValueDrawer.Setup(new DrawerInfoGeneric(
                // index
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
                ValueType
                )
            );

            AddKeyContainer.Add(AddKeyDrawer.VisualElement);
            AddValueContainer.Add(AddValueDrawer.VisualElement);

            // add btn
            AddElementBtn.RegisterCallback<ClickEvent>(HandleAddElementClick);
        }

        private void Refresh()
        {
            IDictionary dict = (IDictionary)Value;

            ElementsContainer.Clear();

            for (int i = 0; i < dict.Count; i++)
            {
                DictionaryElementDrawer elem = new DictionaryElementDrawer();
                elem.Setup(dict, i, KeyType, ValueType);

                ElementsContainer.Add(elem.VisualElement);
            }
        }

        private void HandleAddElementClick(ClickEvent evt)
        {
            IDictionary dict = (IDictionary) Value;
            dict.Add(AddKeyDrawer.Value , AddValueDrawer.Value);
            Refresh();
        }

        public void Clean()
        {

        }
    }
}