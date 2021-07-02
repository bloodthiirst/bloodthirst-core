using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class PortBaseElement
    {

        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/Port/PortBaseElement.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/Port/PortBaseElement.uss";
        private Color color;

        public event Action<PortBaseElement, ClickEvent> OnPortClicked;

        public event Action<PortBaseElement, ContextClickEvent> OnPortRightClicked;

        private VisualElement PortRoot { get; set; }
        private VisualElement PortColor => PortRoot.Q<VisualElement>(nameof(PortColor));
        private VisualElement PortBG => PortRoot.Q<VisualElement>(nameof(PortBG));
        private VisualElement PortSelected => PortRoot.Q<VisualElement>(nameof(PortSelected));

        public VisualElement VisualElement => PortRoot;

        public NodeBaseElement ParentNode { get; }
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

            // color
            Color = NodeEditorUtils.GetColor(portType.PortType);

            // add class to flip the appearance
            string classForDirection = PortType.NodeDirection == NODE_DIRECTION.INPUT ? "input" : "output";
            PortColor.AddToClassList(classForDirection);
            PortBG.AddToClassList(classForDirection);
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

    }
}
