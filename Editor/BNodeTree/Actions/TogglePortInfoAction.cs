namespace Bloodthirst.Editor.BNodeTree
{
    public class TogglePortInfoAction : NodeEditorActionBase
    {
        private PortBaseElement LastToggledPort { get; set; }

        public override void OnDisable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortToggleInfo>(HandleNodeRightClick);
        }

        public override void OnEnable()
        {
            NodeEditor.BEventSystem.Unlisten<OnPortToggleInfo>(HandleNodeRightClick);
            NodeEditor.BEventSystem.Unlisten<OnPortToggleInfo>(HandleNodeRightClick);
        }

        private void HandleNodeRightClick(OnPortToggleInfo evt)
        {
            if (LastToggledPort == evt.PortToggled)
            {
                if (LastToggledPort.IsShowingInfo)
                {
                    LastToggledPort.HideInfo();
                }
                else
                {
                    LastToggledPort.ShowInfo();
                }

                return;
            }


            if (LastToggledPort != null)
            {
                LastToggledPort.HideInfo();
            }

            evt.PortToggled.ShowInfo();
            LastToggledPort = evt.PortToggled;

        }
    }
}
