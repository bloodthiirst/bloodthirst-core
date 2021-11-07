using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class AddNodeAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnCanvasMouseContextClick -= HandleRightMouseClick;
        }

        public override void OnEnable()
        {
            NodeEditor.OnCanvasMouseContextClick -= HandleRightMouseClick;
            NodeEditor.OnCanvasMouseContextClick += HandleRightMouseClick;
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

        private void HandleRightMouseClick(ContextClickEvent evt)
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
                            MousePosition = evt.mousePosition,
                            NodeCreationInfo = kv.Value
                        }
                    );
            }

            Rect menuRect = new Rect(evt.mousePosition, Vector2.zero);
            menu.DropDown(menuRect);
        }

    }
}
