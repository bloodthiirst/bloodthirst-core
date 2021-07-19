using System;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class AddPortOutputAction : NodeEditorActionBase
    {
        public override void OnDisable()
        {
            NodeEditor.OnNodeAddOutput -= HandleNodeAddInput;
        }
        public override void OnEnable()
        {
            NodeEditor.OnNodeAddOutput -= HandleNodeAddInput;
            NodeEditor.OnNodeAddOutput += HandleNodeAddInput;
        }

        private void HandleNodeAddInput(NodeBaseElement node)
        {
            //throw new NotImplementedException();
        }
    }
}
