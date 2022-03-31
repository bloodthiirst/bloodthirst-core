using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BRecorder;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class BRecorderCursorUIBase : VisualElement
    {
        public enum CURSOR_TYPE
        {
            COMMAND,
            CURRENT_TIME
        }

        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/Commands/Cursor/BRecorderCursorUI.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BRecorder/UI/Commands/Cursor/BRecorderCursorUI.uss";

        private CURSOR_TYPE cursorType;
        public CURSOR_TYPE CurorType
        {
            get => cursorType;
            set
            {
                cursorType = value;
                SetStyling(cursorType);
            }
        }

        private void SetStyling(CURSOR_TYPE cursorType)
        {
            switch (cursorType)
            {
                case CURSOR_TYPE.COMMAND:
                    {
                        RemoveFromClassList("current-time-cursor");
                        AddToClassList("command-cursor");
                        break;
                    }
                case CURSOR_TYPE.CURRENT_TIME:
                    {
                        AddToClassList("current-time-cursor");
                        RemoveFromClassList("command-cursor");
                        break;
                    }
            }
        }

        public BRecorderCursorUIBase(CURSOR_TYPE cursorType)
        {
            CurorType = cursorType;

            VisualTreeAsset ui = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
            StyleSheet stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);

            ui.CloneTree(this);
            styleSheets.Add(stylesheet);

            AddToClassList("cursor-base-line");

            style.position = new StyleEnum<Position>(Position.Absolute);
        }

    }
}
