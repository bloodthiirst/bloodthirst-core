using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BRecorder;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class BRecorderCommandCursorUI : VisualElement
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/Commands/Cursor/BRecorderCommandCursorUI.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/Commands/Cursor/BRecorderCommandCursorUI.uss";

        public IBRecorderCommand Command { get; private set; }

        public BRecorderCommandCursorUI(IBRecorderCommand command)
        {
            Command = command;

            VisualTreeAsset ui = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);
            
            ui.CloneTree(this);
            styleSheets.Add(stylesheet);

            AddToClassList("cursor-line");

            style.position = new StyleEnum<Position>(Position.Absolute);
        }

    }
}
