using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class ResetCanvasViewAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnWindowKeyPressed -= HandleKeyDown;
        }

        public override void OnEnable()
        {
            NodeEditor.OnWindowKeyPressed -= HandleKeyDown;
            NodeEditor.OnWindowKeyPressed += HandleKeyDown;
        }

        private void HandleKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode != UnityEngine.KeyCode.R)
                return;

            List<NodeBaseElement> asList = NodeEditor.AllNodes;

            NodeEditor.Zoom = 1;

            Vector2 p = Vector2.zero;

            foreach (NodeBaseElement n in asList)
            {
                p += n.VisualElement.localBound.position;
            }

            p /= asList.Count;

            NodeEditor.PanningOffset = -p;
        }

    }
}
