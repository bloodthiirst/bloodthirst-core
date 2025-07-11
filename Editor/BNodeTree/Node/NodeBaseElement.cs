using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Sirenix.OdinInspector.Editor;
using NUnit.Framework;

namespace Bloodthirst.Editor.BNodeTree
{
    public class NodeBaseElement
    {
        #region consts
        private const string UXML_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Node/NodeBaseElement.uxml";
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Node/NodeBaseElement.uss";
        private const string NODE_IS_SELECTED_USS_CLASS = "is-selected";
        private const string NODE_IS_ACTIVE_USS_CLASS = "is-active";
        #endregion


        #region ui elements
        private VisualElement NodeRoot { get; set; }
        private VisualElement NodeContent => NodeRoot.Q<VisualElement>(nameof(NodeContent));
        private VisualElement NodeHeader => NodeRoot.Q<VisualElement>(nameof(NodeHeader));
        private Label NodeName => NodeRoot.Q<Label>(nameof(NodeName));
        private VisualElement BorderActive => NodeRoot.Q<VisualElement>(nameof(BorderActive));
        private VisualElement BorderSelected => NodeRoot.Q<VisualElement>(nameof(BorderSelected));
        private VisualElement InputPortsContainer => NodeRoot.Q<VisualElement>(nameof(InputPortsContainer));
        private VisualElement OutputPortsContainer => NodeRoot.Q<VisualElement>(nameof(OutputPortsContainer));
        private VisualElement FieldsContainer => NodeRoot.Q<VisualElement>(nameof(FieldsContainer));
        private VisualElement NodeActive => NodeRoot.Q<VisualElement>(nameof(NodeActive));
        private VisualElement NodeResize => NodeRoot.Q<VisualElement>(nameof(NodeResize));
        private Button AddInput => NodeRoot.Q<Button>(nameof(AddInput));
        private Button AddOutput => NodeRoot.Q<Button>(nameof(AddOutput));
        public VisualElement VisualElement => NodeRoot;

        public List<PortBaseElement> Ports { get; }
        public List<IValueDrawer> BindableUIs { get; }
        #endregion

        public bool IsInsideResize { get; set; }
        public bool CanResize { get; set; }
        public bool CanDrag { get; set; }
        public bool IsNodeSelected { get; set; }
        public bool IsNodeActive { get; set; }
        public INodeType NodeType { get; }
        public INodeEditor NodeEditor { get; }

        private PropertyTree odinTree;
        private IMGUIContainer odinDrawer;

        public Vector2 NodeSize
        {
            get
            {
                return new Vector2(NodeRoot.resolvedStyle.width, NodeRoot.resolvedStyle.height);
            }
            set
            {
                NodeRoot.RegisterCallback<GeometryChangedEvent>(OnContentSizeChanged);
                NodeRoot.style.width = new StyleLength(value.x);
                NodeRoot.style.height = new StyleLength(value.y);
                NodeRoot.MarkDirtyRepaint();
            }
        }

        private void OnContentSizeChanged(GeometryChangedEvent geometryChangedEvent)
        {
            NodeRoot.RegisterCallback<GeometryChangedEvent>(OnContentSizeChanged);

            float minWidth = NodeContent.resolvedStyle.width;
            float minHeight = NodeContent.resolvedStyle.height;

            float w = Mathf.Max(NodeSize.x, minWidth);
            float h = Mathf.Max(NodeSize.y, minHeight);

            NodeRoot.style.width = w;
            NodeRoot.style.height = h;

            NodeRoot.MarkDirtyRepaint();

            NodeEditor.BEventSystem.Trigger(new OnNodeResized(NodeEditor, this));
        }

        public NodeBaseElement(INodeType nodeType, INodeEditor nodeEditor)
        {
            NodeType = nodeType;
            NodeEditor = nodeEditor;

            // all
            Ports = new List<PortBaseElement>();

            // ui feilds
            BindableUIs = new List<IValueDrawer>();

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.Instantiate();

            NodeRoot = templateContainer.Q<VisualElement>(nameof(NodeRoot));
            NodeRoot.styleSheets.Add(customUss);

            BorderActive.pickingMode = PickingMode.Ignore;
            BorderSelected.pickingMode = PickingMode.Ignore;


            // node name
            NodeNameAttribute nameAttr = NodeType.GetType().GetCustomAttribute<NodeNameAttribute>();
            NodeName.text = nameAttr == null ? NodeType.GetType().Name : nameAttr.Name;

            //SetupFields();

            odinTree = PropertyTree.Create(nodeType);

            odinDrawer = new IMGUIContainer();
            odinDrawer.onGUIHandler = () =>
            {
                InspectorProperty inspectorProperty = odinTree.RootProperty;
                odinTree.Draw(false);
                odinTree.UpdateTree();
            };

            FieldsContainer.Add(odinDrawer);
        }


        public void AddPort(IPortType curr)
        {
            PortBaseElement input = new PortBaseElement(NodeEditor, this, curr);

            Ports.Add(input);

            VisualElement container = null;
            if (curr.PortDirection == PORT_DIRECTION.INPUT)
            {
                container = InputPortsContainer;
            }
            else
            {
                container = OutputPortsContainer;
            }

            Assert.IsNotNull(container);

            container.Add(input.VisualElement);

            input.AfterAddToCanvas();
        }

        public void RemovePort(IPortType port)
        {
            PortBaseElement curr = Ports.FirstOrDefault(p => p.PortType == port);

            if (curr == null)
            {
                throw new Exception("Port not found when trying to remove");
            }

            VisualElement container = null;

            if (port.PortDirection == PORT_DIRECTION.INPUT)
            {
                container = InputPortsContainer;
            }
            else
            {
                container = OutputPortsContainer;
            }

            curr.BeforeRemoveFromCanvas();

            container.Remove(curr.VisualElement);

            port.ParentNode.RemovePort(port);

            Ports.Remove(curr);
        }

        public void RefreshPorts()
        {
            // clear all ports
            ClearAllPorts();

            // readd the ports
            AddAllPorts();
        }

        private void AddAllPorts()
        {
            // input ports
            foreach (IPortType p in NodeType.Ports)
            {
                AddPort(p);
            }
        }

        private void ClearAllPorts()
        {
            // input ports
            for (int i = NodeType.Ports.Count - 1; i >= 0; i--)
            {
                IPortType p = NodeType.Ports[i];
                RemovePort(p);
            }
        }

        public void BeforeAddToCanvas()
        {
            AddAllPorts();
        }

        public void AfterAddToCanvas()
        {
            // node events
            NodeType.OnPortAdded -= HandleAddPort;
            NodeType.OnPortAdded += HandleAddPort;

            // adding ports
            AddInput.clickable.clicked -= HandleAddInput;
            AddInput.clickable.clicked += HandleAddInput;

            AddOutput.clickable.clicked -= HandleAddOutput;
            AddOutput.clickable.clicked += HandleAddOutput;

            // moving
            NodeHeader.RegisterCallback<MouseDownEvent>(OnMouseDown);
            NodeHeader.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            NodeHeader.RegisterCallback<MouseUpEvent>(OnMouseUp);
            NodeHeader.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            // click
            NodeRoot.RegisterCallback<ClickEvent>(OnClick);
            NodeRoot.RegisterCallback<ContextClickEvent>(OnRightClick);

            //resizing
            NodeResize.RegisterCallback<MouseEnterEvent>(OnResizeEnter);
            NodeResize.RegisterCallback<MouseLeaveEvent>(OnResizeLeave);
            NodeResize.RegisterCallback<MouseDownEvent>(OnResizeDown);
            NodeResize.RegisterCallback<MouseUpEvent>(OnResizeUp);

            // sub ports
            foreach (PortBaseElement p in Ports)
            {
                p.AfterAddToCanvas();
            }
        }

        private void HandleAddOutput()
        {
            NodeEditor.BEventSystem.Trigger(new OnPortRequestAddOutput(NodeEditor, this));
        }

        private void HandleAddInput()
        {
            NodeEditor.BEventSystem.Trigger(new OnPortRequestAddInput(NodeEditor, this));
        }

        private void HandleAddPort(IPortType port)
        {
            AddPort(port);
        }

        private void OnResizeUp(MouseUpEvent evt)
        {
            CanResize = false;

            NodeEditor.BEventSystem.Trigger(new OnNodeEndResize(NodeEditor, this));
        }

        private void OnResizeDown(MouseDownEvent evt)
        {
            CanResize = true;

            NodeEditor.BEventSystem.Trigger(new OnNodeStartResize(NodeEditor, this));
        }
        private void OnResizeLeave(MouseLeaveEvent evt)
        {
            IsInsideResize = false;
            CanDrag = false;

            NodeEditor.BEventSystem.Trigger(new OnNodeEndResize(NodeEditor, this));
        }

        private void OnResizeEnter(MouseEnterEvent evt)
        {
            IsInsideResize = true;
        }

        public void BeforeRemoveFromCanvas()
        {
            odinDrawer.Dispose();
            odinTree.Dispose();

            // node events
            NodeType.OnPortAdded -= HandleAddPort;
            // moving
            NodeHeader.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            NodeHeader.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            NodeHeader.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            NodeHeader.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);

            AddInput.clickable.clicked -= HandleAddInput;

            NodeRoot.UnregisterCallback<ClickEvent>(OnClick);
            NodeRoot.UnregisterCallback<ContextClickEvent>(OnRightClick);

            //resizing
            NodeResize.UnregisterCallback<MouseEnterEvent>(OnResizeEnter, TrickleDown.NoTrickleDown);
            NodeResize.UnregisterCallback<MouseLeaveEvent>(OnResizeLeave, TrickleDown.NoTrickleDown);
            NodeResize.UnregisterCallback<MouseDownEvent>(OnResizeDown, TrickleDown.NoTrickleDown);
            NodeResize.UnregisterCallback<MouseUpEvent>(OnResizeUp, TrickleDown.NoTrickleDown);

            foreach (PortBaseElement p in Ports)
            {
                p.BeforeRemoveFromCanvas();
            }
        }

        public void AfterRemoveFromCanvs()
        {
            ClearAllPorts();

            foreach (IValueDrawer b in BindableUIs)
            {
                b.Destroy();
            }
        }

        #region canvas events
        private void OnRightClick(ContextClickEvent evt)
        {
            NodeEditor.BEventSystem.Trigger(new OnNodeMouseContextClick(NodeEditor, this, evt));
        }

        private void OnClick(ClickEvent evt)
        {
            NodeEditor.BEventSystem.Trigger(new OnNodeMouseClick(NodeEditor, this, evt));
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (evt.button != 0)
                return;

            CanDrag = false;
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (evt.button != 0)
                return;

            if (!CanDrag)
                return;

            Vector3 res = NodeRoot.parent.worldTransform.inverse.MultiplyVector(evt.mouseDelta);

            NodeRoot.transform.position += res;

            NodeEditor.BEventSystem.Trigger(new OnNodeMoved(NodeEditor, this));
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 0)
                return;

            NodeRoot.BringToFront();

            CanDrag = true;
        }
        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            CanDrag = false;
        }
        #endregion

        public void SelectInCanvas()
        {
            IsNodeSelected = true;
            NodeRoot.AddToClassList(NODE_IS_SELECTED_USS_CLASS);
        }

        public void DeselectInCanvas()
        {
            IsNodeSelected = false;
            NodeRoot.RemoveFromClassList(NODE_IS_SELECTED_USS_CLASS);
        }

        public void SelectActive()
        {
            IsNodeActive = true;
            NodeRoot.AddToClassList(NODE_IS_ACTIVE_USS_CLASS);
        }

        public void DeselectActive()
        {
            IsNodeActive = false;
            NodeRoot.RemoveFromClassList(NODE_IS_ACTIVE_USS_CLASS);
        }
    }
}
