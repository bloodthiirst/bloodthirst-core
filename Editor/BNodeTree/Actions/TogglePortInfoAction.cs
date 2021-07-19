namespace Bloodthirst.System.Quest.Editor
{
    public class TogglePortInfoAction : NodeEditorActionBase
    {
        private PortBaseElement LastToggledPort { get; set; }

        public override void OnDisable()
        {
            NodeEditor.OnPortToggleInfo -= HandleNodeRightClick;
        }

        public override void OnEnable()
        {
            NodeEditor.OnPortToggleInfo -= HandleNodeRightClick;
            NodeEditor.OnPortToggleInfo += HandleNodeRightClick;
        }

        private void HandleNodeRightClick(PortBaseElement rightClickedPort)
        {
            if (LastToggledPort == rightClickedPort)
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

            rightClickedPort.ShowInfo();
            LastToggledPort = rightClickedPort;

        }
    }
}
