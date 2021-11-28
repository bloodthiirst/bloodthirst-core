using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class EnumElementDrawer
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Enum/Element/EnumElementDrawer.uxml";
        private const string PATH_USS =     "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Enum/Element/EnumElementDrawer.uss";

        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);

        public IDrawerInfo DrawerInfo { get; private set; }
        public object Value { get; set; } = 0;
        public Type EnumType { get; private set; }
        public VisualElement VisualElement { get; private set; }
        public Label EnumNameLabel { get; private set; }
        public string EnumName { get; private set; }
        public int EnumValue { get; private set; }

        public EnumElementDrawer()
        {

            // ui setup
            VisualElement ui = uxmlAsset.CloneTree();
            ui.styleSheets.Add(ussAsset);
            VisualElement = ui;
        }

        public void Setup(string enumName , int enumValue)
        {
            EnumName = enumName;
            EnumValue = enumValue;

            EnumNameLabel = VisualElement.Q<Label>(nameof(EnumNameLabel));

            EnumNameLabel.text = enumName;

        }
    }
}