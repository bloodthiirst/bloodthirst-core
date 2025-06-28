using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BNodeTree;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor;
using Sirenix.OdinInspector.Editor;


namespace Bloodthirst.Editor.BNodeTree
{
    public class PortInfoBaseElement
    {
        private const string UXML_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/PortInfo/PortInfoBaseElement.uxml";
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/PortInfo/PortInfoBaseElement.uss";
        private IMGUIContainer odinDrawer;

        private VisualElement PortInfoRoot { get; set; }
        public VisualElement VisualElement => PortInfoRoot;
        private VisualElement BorderActive => PortInfoRoot.Q<VisualElement>(nameof(BorderActive));
        private VisualElement BorderSelected => PortInfoRoot.Q<VisualElement>(nameof(BorderSelected));
        private VisualElement FieldsContainer => PortInfoRoot.Q<VisualElement>(nameof(FieldsContainer));
        private Label NoFieldsAvailable => PortInfoRoot.Q<Label>(nameof(NoFieldsAvailable));
        private Label PortName => PortInfoRoot.Q<Label>(nameof(PortName));

        public PortBaseElement PortBase { get; }
        public IPortType PortType { get; set; }

        public bool IsShowingInfo { get; private set; }

        public PortInfoBaseElement(PortBaseElement port, IPortType portType)
        {
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.CloneTree();
            PortInfoRoot = templateContainer.Q<VisualElement>(nameof(PortInfoRoot));
            PortInfoRoot.styleSheets.Add(customUss);

            BorderActive.pickingMode = PickingMode.Ignore;
            BorderSelected.pickingMode = PickingMode.Ignore;

            // parent node ui
            PortBase = port;

            // port instance
            PortType = portType;

            PortName.text = PortType.PortName;



            NoFieldsAvailable.style.display = DisplayStyle.None;
            FieldsContainer.style.display = DisplayStyle.Flex;


            PropertyTree newTree = PropertyTree.Create(portType);

            odinDrawer = new IMGUIContainer();
            odinDrawer.onGUIHandler = () =>
            {
                InspectorProperty inspectorProperty = newTree.RootProperty;
                newTree.Draw(false);
                newTree.UpdateTree();
            };

            FieldsContainer.Add(odinDrawer);

            PortInfoRoot.MarkDirtyRepaint();
        }

        public void AfterAddToCanvas()
        {
            PortInfoRoot.RegisterCallback<ClickEvent>(OnClick);
            PortInfoRoot.RegisterCallback<ContextClickEvent>(OnRightClick);
        }

        public void BeforeRemoveFromCanvas()
        {
            odinDrawer.Dispose();
            PortInfoRoot.UnregisterCallback<ClickEvent>(OnClick);
            PortInfoRoot.UnregisterCallback<ContextClickEvent>(OnRightClick);
        }

        private void OnRightClick(ContextClickEvent evt)
        {

        }

        private void OnClick(ClickEvent evt)
        {

        }
    }
}
