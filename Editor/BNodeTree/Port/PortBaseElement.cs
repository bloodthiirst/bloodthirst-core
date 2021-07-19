using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class PortBaseElement
    {

        private const string UXML_PATH =  BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Port/PortBaseElement.uxml";
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Port/PortBaseElement.uss";
        private const string PORT_INFO_IS_OPEN_CLASS = "is-open";


        private Color color;

        public event Action<PortBaseElement, ClickEvent> OnPortClicked;

        public event Action<PortBaseElement, ContextClickEvent> OnPortRightClicked;

        public event Action<PortBaseElement> OnPortToggleInfoDialog;

        private VisualElement PortRoot { get; set; }
        private VisualElement PortColor => PortRoot.Q<VisualElement>(nameof(PortColor));
        private Label PortName => PortRoot.Q<Label>(nameof(PortName));
        private VisualElement PortBG => PortRoot.Q<VisualElement>(nameof(PortBG));
        private VisualElement PortSelected => PortRoot.Q<VisualElement>(nameof(PortSelected));
        private VisualElement PortInfoContainer => PortRoot.Q<VisualElement>(nameof(PortInfoContainer));
        public VisualElement VisualElement => PortRoot;

        public NodeBaseElement ParentNode { get; }
        public PortInfoBaseElement PortInfo { get; }
        public LinkElement Link { get; set; }
        public IPortType PortType { get; set; }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                PortColor.style.backgroundColor = new StyleColor(color);
                PortSelected.style.backgroundColor = new StyleColor(color);
            }
        }

        public bool IsShowingInfo { get; private set; }

        public PortBaseElement( NodeBaseElement parentNode, IPortType portType)
        {
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.CloneTree();
            PortRoot = templateContainer.Q<VisualElement>(nameof(PortRoot));
            PortRoot.styleSheets.Add(customUss);

            // parent node ui
            ParentNode = parentNode;

            // port instance
            PortType = portType;

            // info 

            PortInfo = new PortInfoBaseElement(this , portType);

            PortInfoContainer.Add(PortInfo.VisualElement);

            // color
            Color = BNodeTreeEditorUtils.GetColor(portType.PortType);

            // label
            PortName.text = PortType.PortName;

            // add class to flip the appearance
            if (PortType.NodeDirection == NODE_DIRECTION.INPUT)
            {
                string classForDirection = "input";
                PortRoot.AddToClassList(classForDirection);
            }
            else
            {
                string classForDirection = "output";
                PortRoot.AddToClassList(classForDirection);
            }
        }

        public void AfterAddToCanvas()
        {
            PortRoot.RegisterCallback<ClickEvent>(OnClick);
            PortRoot.RegisterCallback<ContextClickEvent>(OnRightClick);
        }

        public void BeforeRemoveFromCanvas()
        {
            PortRoot.UnregisterCallback<ClickEvent>(OnClick);
            PortRoot.UnregisterCallback<ContextClickEvent>(OnRightClick);
            OnPortClicked = null;
            OnPortRightClicked = null;
        }

        private void OnRightClick(ContextClickEvent evt)
        {
            OnPortRightClicked?.Invoke(this, evt);
        }

        private void OnClick(ClickEvent evt)
        {
            
            if (evt.target != PortName)
                return;

            OnPortToggleInfoDialog?.Invoke(this);

            OnPortClicked?.Invoke(this, evt);
        }

        public void Select()
        {
            PortRoot.AddToClassList("selected");
        }
        public void Deselect()
        {
            PortRoot.RemoveFromClassList("selected");
        }
        public void ShowInfo()
        {
            IsShowingInfo = true;
            PortRoot.AddToClassList(PORT_INFO_IS_OPEN_CLASS);
        }
        public void HideInfo()
        {
            IsShowingInfo = false;
            PortRoot.RemoveFromClassList(PORT_INFO_IS_OPEN_CLASS);
        }

    }
}
