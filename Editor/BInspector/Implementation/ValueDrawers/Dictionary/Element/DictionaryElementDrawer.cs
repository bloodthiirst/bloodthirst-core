using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;

namespace Bloodthirst.Editor.BInspector
{
    public class DictionaryElementDrawer
    {
        private const string PATH_UXML =    "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Dictionary/Element/DictionaryElementDrawer.uxml";
        private const string PATH_USS =     "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Dictionary/Element/DictionaryElementDrawer.uss";
        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);

        public VisualElement VisualElement { get; private set; }
        public VisualElement KeyContainer { get; private set; }
        public VisualElement ValueContainer { get; private set; }
        public IValueDrawerInfo DrawerInfo { get; private set; }
        public Type KeyType { get; private set; }
        public Type ValueType { get; private set; }
        public IDictionary Dictionary { get; private set; }
        public IValueDrawer KeyDrawer { get; private set; }
        public IValueDrawer ValueDrawer { get; private set; }
        public int Index { get; private set; }

        public object Key { get; private set; }
        public object Value { get; private set; }

        public object DefaultValue()
        {
            return null;
        }

        public void Setup(IDictionary dictionary, int Index, Type keyType, Type valueType , DrawerContext drawerContext)
        {
            this.KeyType = keyType;
            this.ValueType = valueType;
            this.Dictionary = dictionary;

            VisualElement ui = uxmlAsset.CloneTree();
            ui.styleSheets.Add(ussAsset);
            VisualElement = ui;

            KeyContainer = VisualElement.Q<VisualElement>(nameof(KeyContainer));
            ValueContainer = VisualElement.Q<VisualElement>(nameof(ValueContainer));

            // setup add element

            KeyDrawer = ValueDrawerProvider.Get(KeyType);
            ValueDrawer = ValueDrawerProvider.Get(ValueType);

            object[] vals = new object[Dictionary.Count];
            object[] keys = new object[Dictionary.Count];


            dictionary.Values.CopyTo(vals, 0);
            dictionary.Keys.CopyTo(keys, 0);

            object key = keys[Index];
            object val = vals[Index];

            Key = key;
            Value = val;

            VisualElement element = uxmlAsset.CloneTree();

            // key setup
            KeyDrawer.Setup( GetKeyDrawerInfo(Index), KeyDrawer, drawerContext );
            // value setup
            ValueDrawer.Setup( GetValueDrawerInfo(Index), ValueDrawer, drawerContext );
            

            KeyContainer.Add(KeyDrawer.DrawerRoot);
            ValueContainer.Add(ValueDrawer.DrawerRoot);
        }

        private ValueDrawerInfoGeneric GetValueDrawerInfo(int Index)
        {
            return new ValueDrawerInfoGeneric(
                                // index
                                Index,
                                // getter
                                () =>
                                {
                                    var c = 0;
                                    foreach (object v in Dictionary.Values)
                                    {
                                        if (c == Index)
                                            return v;
                                        c++;
                                    }

                                    return null;
                                },
                                // setter
                                (newValue) =>
                                {
                                    var c = 0;
                                    foreach (object k in Dictionary.Keys)
                                    {
                                        if (c == Index)
                                        {
                                            Dictionary[k] = newValue;
                                            return;
                                        }
                                        c++;
                                    }

                                },
                                // type
                                ValueType,

                                // parent
                                Dictionary
                                );
        }

        private ValueDrawerInfoGeneric GetKeyDrawerInfo(int Index)
        {
            return new ValueDrawerInfoGeneric(

                                // index
                                Index,
                                // getter
                                () =>
                                {
                                    var c = 0;
                                    foreach (object k in Dictionary.Keys)
                                    {
                                        if (c == Index)
                                            return k;
                                        c++;
                                    }

                                    return null;
                                },
                                // setter
                                (newKey) =>
                                {
                                    var c = 0;
                                    foreach (object k in Dictionary.Keys)
                                    {
                                        if (c == Index)
                                        {
                                            var oldVal = Dictionary[k];
                                            Dictionary.Remove(k);
                                            Dictionary.Add(newKey, oldVal);
                                            return;
                                        }
                                        c++;
                                    }

                                },
                                // type
                                KeyType,

                                // parent
                                Dictionary
                                );
        }

        public void Clean()
        {

        }
    }
}