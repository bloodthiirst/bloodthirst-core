using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class BuildNodeTreeAction : NodeEditorActionBase
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

           //NodeEditor.GetRootNodes();
        }

    }
}
