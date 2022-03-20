using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class PortBaseElement
    {

        private const string UXML_PATH =  BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Port/PortBaseElement.uxml";
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Port/PortBaseElement.uss";
        private const string PORT_INFO_IS_OPEN_CLASS = "is-open";


        private Color color;
        private double lastClickTimestamp;
        private EditorCoroutine crtHandle;

        private VisualElement PortRoot { get; set; }
        private VisualElement PortColor => PortRoot.Q<VisualElement>(nameof(PortColor));
        private Label PortName => PortRoot.Q<Label>(nameof(PortName));
        private TextField PortNameEdit => PortRoot.Q<TextField>(nameof(PortNameEdit));
        private VisualElement PortBG => PortRoot.Q<VisualElement>(nameof(PortBG));
        private VisualElement PortSelected => PortRoot.Q<VisualElement>(nameof(PortSelected));
        private VisualElement PortInfoContainer => PortRoot.Q<VisualElement>(nameof(PortInfoContainer));
        public VisualElement VisualElement => PortRoot;

        public INodeEditor NodeEditor { get; }
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

        public PortBaseElement(INodeEditor nodeEditor, NodeBaseElement parentNode, IPortType portType)
        {
            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.CloneTree();
            PortRoot = templateContainer.Q<VisualElement>(nameof(PortRoot));
            PortRoot.styleSheets.Add(customUss);

            NodeEditor = nodeEditor;

            // parent node ui
            ParentNode = parentNode;

            // port instance
            PortType = portType;

            // info 
            PortInfo = new PortInfoBaseElement(this , portType);

            PortInfoContainer.Add(PortInfo.VisualElement);

            // color
            Color = BNodeTreeEditorUtils.GetColor(portType.PortValueType);

            // label
            PortName.text = PortType.PortName;

            //edit
            //PortNameEdit.labelElement.parent.Remove(PortNameEdit.labelElement);
            PortNameEdit.labelElement.Display(false);

            PortName.Display(true);
            PortNameEdit.Display(false);
            PortNameEdit.focusable = false;

            // add class to flip the appearance
            if (PortType.PortDirection == PORT_DIRECTION.INPUT)
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
            PortRoot.RegisterCallback<MouseDownEvent>(OnClick);
            PortRoot.RegisterCallback<ContextClickEvent>(OnRightClick);

            PortNameEdit.RegisterCallback<KeyDownEvent>(HandleKeydown);
        }


        public void BeforeRemoveFromCanvas()
        {
            PortRoot.UnregisterCallback<MouseDownEvent>(OnClick);
            PortRoot.UnregisterCallback<ContextClickEvent>(OnRightClick);

            PortNameEdit.UnregisterCallback<KeyDownEvent>(HandleKeydown);
        }

        private void HandleKeydown(KeyDownEvent evt)
        {
            if(evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                PortType.PortName = PortNameEdit.value;
                PortName.text = PortNameEdit.value;
                PortName.Display(true);
                PortNameEdit.Display(false);
                PortNameEdit.focusable = false;
            }

            if(evt.keyCode == KeyCode.Escape)
            {
                PortName.Display(true);
                PortNameEdit.Display(false);
                PortNameEdit.focusable = false;
            }
        }


        private void OnRightClick(ContextClickEvent evt)
        {
            NodeEditor.BEventSystem.Trigger(new OnPortMouseContextClick(NodeEditor , this , evt));
        }

        IEnumerator CrtDoubleClick(double clickTime)
        {
            double waitTime = 0.3f;
            yield return new WaitUntil(() => EditorApplication.timeSinceStartup >= clickTime + waitTime);
            double delta = lastClickTimestamp - clickTime;

            // if one click
            if (delta < 0.001f)
            {
                NodeEditor.BEventSystem.Trigger(new OnPortToggleInfo(NodeEditor, this)); ;
            }
            // else two clicks
            else
            {
                PortNameEdit.focusable = true;
                PortNameEdit.Focus();
                PortNameEdit.value = PortType.PortName;
                PortName.Display(false);
                PortNameEdit.Display(true);
            }

    

            crtHandle = null;
        }

        private void OnClick(MouseDownEvent evt)
        {   
            if (evt.target != PortName)
                return;

            lastClickTimestamp = EditorApplication.timeSinceStartup;

            if (crtHandle == null)
            {
                crtHandle = EditorCoroutineUtility.StartCoroutine(CrtDoubleClick(EditorApplication.timeSinceStartup), this);
            }

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
