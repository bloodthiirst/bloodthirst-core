using Bloodthirst.Runtime.BNodeTree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class AddNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseContextClick>(HandleRightMouseClick);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnCanvasMouseContextClick>(HandleRightMouseClick);
            NodeEditor.BEventSystem.Listen<OnCanvasMouseContextClick>(HandleRightMouseClick);
        }

        private void ContextMenuAddNode(object menuItem)
        {
            NodeCreationData data = (NodeCreationData)menuItem;

            INodeType node = data.NodeCreationInfo.NodeCreator();

            NodeEditor.AddNode(node, data.MousePosition , null);
        }

        private struct NodeCreationData
        {
            public Vector2 MousePosition { get; set; }
            public NodeProvider.FactoryRecord NodeCreationInfo { get; set; }
        }

        private void HandleRightMouseClick(OnCanvasMouseContextClick evt)
        {
            GenericMenu menu = new GenericMenu();

            foreach (var kv in NodeProvider.GetNodeFactory(NodeEditor.NodeBaseType))
            {
                menu.AddItem
                    (
                        new GUIContent("Add Node/" + kv.Value.NodePath),
                        false,
                        ContextMenuAddNode,
                        new NodeCreationData()
                        {
                            MousePosition = evt.ClickEvent.mousePosition,
                            NodeCreationInfo = kv.Value
                        }
                    );
            }

            Rect menuRect = new Rect(evt.ClickEvent.mousePosition, Vector2.zero);
            menu.DropDown(menuRect);
        }

    }
}
