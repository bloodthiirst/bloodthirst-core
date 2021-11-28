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
    public class ScriptObjectFieldDrawer
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/ScriptObjectFieldDrawer/ScriptObjectFieldDrawer.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/ScriptObjectFieldDrawer/ScriptObjectFieldDrawer.uss";

        private static readonly VisualTreeAsset uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
        private static readonly StyleSheet ussAsset = AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);

        private const string ScriptNameLabel = nameof(ScriptNameLabel);
        private const string ScriptIconImage = nameof(ScriptIconImage);
        private const string ScriptSelectionZone = nameof(ScriptSelectionZone);

        public VisualElement CreateVisualElement(MonoScript script)
        {

            TemplateContainer ui = uxmlAsset.CloneTree();

            Label name = ui.Q<Label>(ScriptNameLabel);
            Image icon = ui.Q<Image>(ScriptIconImage);
            VisualElement clickZone = ui.Q<VisualElement>(ScriptSelectionZone);
            clickZone.userData = script;

            string path = AssetDatabase.GetAssetPath(script);
            Texture iconTexture = AssetDatabase.GetCachedIcon(path);

            icon.image = iconTexture;
            name.text = script.GetClass().GetNiceName();
            clickZone.RegisterCallback<ClickEvent>(HandleClickOnScript);

            ui.styleSheets.Add(ussAsset);

            return ui;
        }

        private void HandleClickOnScript(ClickEvent evt)
        {
            MonoScript script = (evt.currentTarget as VisualElement).userData as MonoScript;

            if (evt.clickCount == 1)
            {
                EditorGUIUtility.PingObject(script);
            }
            if(evt.clickCount == 2)
            {
                AssetDatabase.OpenAsset(script);
            }
        }
    }
}