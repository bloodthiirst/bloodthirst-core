using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class NodeBaseElement
    {
        #region consts
        private const string UXML_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Node/NodeBaseElement.uxml";
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Node/NodeBaseElement.uss";
        private const string NODE_IS_SELECTED_USS_CLASS = "is-selected";
        private const string NODE_IS_ACTIVE_USS_CLASS = "is-active";
        #endregion

        #region events
        public event Action<NodeBaseElement, ClickEvent> OnNodeClicked;
        public event Action<NodeBaseElement, ContextClickEvent> OnNodeRightClicked;
        public event Action<NodeBaseElement, ClickEvent> OnNodeRequestRemove;
        public event Action<NodeBaseElement> OnNodeMoved;

        public event Action<NodeBaseElement> OnNodeStartResize;
        public event Action<NodeBaseElement> OnNodeEndResize;
        public event Action<NodeBaseElement> OnNodeResized;

        public event Action<NodeBaseElement> OnRequestNodeAddInput;
        public event Action<NodeBaseElement> OnRequestNodeAddOutput;

        public event Action<NodeBaseElement, PortBaseElement> OnNodePortAdded;

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
        public List<IBindableUI> BindableUIs { get; }
        #endregion

        public bool IsInsideResize { get; set; }
        public bool CanResize { get; set; }
        public bool CanDrag { get; set; }
        public bool IsNodeSelected { get; set; }
        public bool IsNodeActive { get; set; }
        public INodeType NodeType { get; }
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
            OnNodeResized?.Invoke(this);
        }

        public NodeBaseElement(INodeType nodeType)
        {
            // all
            Ports = new List<PortBaseElement>();

            // ui feilds
            BindableUIs = new List<IBindableUI>();

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.Instantiate();

            NodeRoot = templateContainer.Q<VisualElement>(nameof(NodeRoot));
            NodeRoot.styleSheets.Add(customUss);

            BorderActive.pickingMode = PickingMode.Ignore;
            BorderSelected.pickingMode = PickingMode.Ignore;

            NodeType = nodeType;

            // node name
            NodeNameAttribute nameAttr = NodeType.GetType().GetCustomAttribute<NodeNameAttribute>();
            NodeName.text = nameAttr == null ? NodeType.GetType().Name : nameAttr.Name;

            SetupFields();

            FixLabels();
        }

        private void FixLabels()
        {
            if (BindableUIs.Count == 0)
                return;

            // max label length
            int maxLabelLength = BindableUIs.Max(l => l.MemberInfo.Name.Length);

            // ratio picked
            float fontRatio = 70 / 10f;

            float labelWidth = fontRatio * maxLabelLength;

            foreach (IBindableUI ui in BindableUIs)
            {
                foreach (Label l in ui.VisualElement.Query<Label>().Build().ToList())
                {
                    l.style.width = new StyleLength(labelWidth);
                }
            }
        }

        /// <summary>
        /// Get all members of the node type
        /// </summary>
        /// <returns></returns>
        private IEnumerable<MemberInfo> GetAllMembers()
        {
            foreach (PropertyInfo f in NodeType.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                yield return f;
            }

            foreach (FieldInfo f in NodeType.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                yield return f;
            }
        }

        /// <summary>
        /// Get the filtred members that should be shown on the node's ui
        /// </summary>
        /// <returns></returns>
        private List<MemberInfo> ValidMembers()
        {
            IEnumerable<MemberInfo> allInterfaceMembers = NodeType.GetType().GetInterfaces().SelectMany(i => i.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            IEnumerable<MemberInfo> members = GetAllMembers()

                .Where(m =>
                {
                    List<MemberInfo> mem = allInterfaceMembers.Where(i => i.Name == m.Name).ToList();

                    if (mem.Count == 0)
                        return true;

                    return mem.FirstOrDefault(im => im != null && im.GetCustomAttribute<IgnoreBindableAttribute>() == null) != null;
                })

                .Where(m => !m.Name.EndsWith("__BackingField"));

            List<MemberInfo> lst = members.ToList();

            return lst.ToList();
        }

        /// <summary>
        /// Create the ui for the fields of the node
        /// </summary>
        private void SetupFields()
        {
            List<MemberInfo> members = ValidMembers();


            foreach (MemberInfo mem in members)
            {
                IBindableUIFactory factory = BindableUIProvider.UIFactory.FirstOrDefault(f => f.CanBind(mem));

                if (factory == null)
                    continue;

                IBindableUI bindable = factory.CreateUI();

                BindableUIs.Add(bindable);

                bindable.Setup(NodeType, mem);
                FieldsContainer.Add(bindable.VisualElement);

            }
        }

        #region add input
        private void AddConstInputPort(IPortType curr)
        {
            PortBaseElement input = new PortBaseElement(this, curr);
            InputPortsContainer.Add(input.VisualElement);

            Ports.Add(input);

            input.AfterAddToCanvas();
            OnNodePortAdded?.Invoke(this, input);
        }
        public void AddVariableInputPort(IPortType curr)
        {
            PortBaseElement input = new PortBaseElement(this, curr);
            InputPortsContainer.Add(input.VisualElement);

            Ports.Add(input);

            input.AfterAddToCanvas();
            OnNodePortAdded?.Invoke(this, input);

        }
        #endregion

        #region add output
        private void AddConstOutputPort(IPortType port)
        {
            PortBaseElement output = new PortBaseElement(this, port);
            OutputPortsContainer.Add(output.VisualElement);

            Ports.Add(output);

            OnNodePortAdded?.Invoke(this, output);
        }

        public void AddVariableOutputPort(IPortType port)
        {
            PortBaseElement output = new PortBaseElement(this, port);
            OutputPortsContainer.Add(output.VisualElement);

            Ports.Add(output);

            output.AfterAddToCanvas();
            OnNodePortAdded?.Invoke(this, output);
        }
        #endregion

        #region remove output
        private void RemoveConstOutputPort(IPortType port)
        {
            PortBaseElement curr = Ports.FirstOrDefault(p => p.PortType == port);

            if (curr == null)
            {
                throw new Exception("Port not found when trying to remove");
            }
            curr.BeforeRemoveFromCanvas();
            OutputPortsContainer.Remove(curr.VisualElement);
            Ports.Remove(curr);
        }

        private void RemoveVariableOutputPort(IPortType port)
        {
            PortBaseElement curr = Ports.FirstOrDefault(p => p.PortType == port);

            if (curr == null)
            {
                throw new Exception("Port not found when trying to remove");
            }

            curr.BeforeRemoveFromCanvas();
            OutputPortsContainer.Remove(curr.VisualElement);
            Ports.Remove(curr);
        }
        #endregion

        #region remove input
        private void RemoveConstInputPort(IPortType port)
        {
            PortBaseElement curr = Ports.FirstOrDefault(p => p.PortType == port);

            if (curr == null)
            {
                throw new Exception("Port not found when trying to remove");
            }

            curr.BeforeRemoveFromCanvas();
            InputPortsContainer.Remove(curr.VisualElement);
            Ports.Remove(curr);
        }

        private void RemoveVariableInputPort(IPortType port)
        {
            PortBaseElement curr = Ports.FirstOrDefault(p => p.PortType == port);

            if (curr == null)
            {
                throw new Exception("Port not found when trying to remove");
            }

            curr.BeforeRemoveFromCanvas();

            Ports.Remove(curr);

            InputPortsContainer.Remove(curr.VisualElement);
        }
        #endregion

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

                if ((p.PortDirection == PORT_DIRECTION.INPUT && p.PortType == PORT_TYPE.CONST))
                {
                    AddConstInputPort(p);
                    continue;
                }
                if ((p.PortDirection == PORT_DIRECTION.INPUT && p.PortType == PORT_TYPE.VARIABLE))
                {
                    AddVariableInputPort(p);
                    continue;
                }
                // output ports 
                if ((p.PortDirection == PORT_DIRECTION.OUTPUT && p.PortType == PORT_TYPE.CONST))
                {
                    AddConstOutputPort(p);
                    continue;
                }
                if ((p.PortDirection == PORT_DIRECTION.OUTPUT && p.PortType == PORT_TYPE.VARIABLE))
                {
                    AddVariableOutputPort(p);
                    continue;
                }
            }
        }

        private void ClearAllPorts()
        {
            // input ports
            foreach (IPortType p in NodeType.GetPorts(PORT_DIRECTION.INPUT, PORT_TYPE.CONST))
            {
                RemoveConstInputPort(p);
            }
            foreach (IPortType p in NodeType.GetPorts(PORT_DIRECTION.INPUT, PORT_TYPE.VARIABLE))
            {
                RemoveVariableInputPort(p);
            }
            // output ports 
            foreach (IPortType p in NodeType.GetPorts(PORT_DIRECTION.OUTPUT, PORT_TYPE.CONST))
            {
                RemoveConstOutputPort(p);
            }
            foreach (IPortType p in NodeType.GetPorts(PORT_DIRECTION.OUTPUT, PORT_TYPE.VARIABLE))
            {
                RemoveVariableOutputPort(p);
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

        private void HandleAddPort(IPortType port)
        {
            if (port.PortDirection == PORT_DIRECTION.INPUT && port.PortType == PORT_TYPE.CONST)
            {
                AddConstInputPort(port);
            }

            if (port.PortDirection == PORT_DIRECTION.INPUT && port.PortType == PORT_TYPE.VARIABLE)
            {
                AddVariableInputPort(port);
            }

            if (port.PortDirection == PORT_DIRECTION.OUTPUT && port.PortType == PORT_TYPE.CONST)
            {
                AddConstOutputPort(port);
            }

            if (port.PortDirection == PORT_DIRECTION.OUTPUT && port.PortType == PORT_TYPE.VARIABLE)
            {
                AddVariableOutputPort(port);
            }
        }

        private void HandleAddOutput()
        {
            OnRequestNodeAddOutput?.Invoke(this);
        }

        private void HandleAddInput()
        {
            OnRequestNodeAddInput?.Invoke(this);
        }

        private void OnResizeUp(MouseUpEvent evt)
        {
            CanResize = false;

            OnNodeEndResize?.Invoke(this);
        }

        private void OnResizeDown(MouseDownEvent evt)
        {
            CanResize = true;

            OnNodeStartResize?.Invoke(this);
        }
        private void OnResizeLeave(MouseLeaveEvent evt)
        {
            IsInsideResize = false;
            CanDrag = false;

            OnNodeEndResize?.Invoke(this);
        }

        private void OnResizeEnter(MouseEnterEvent evt)
        {
            IsInsideResize = true;
        }

        public void BeforeRemoveFromCanvas()
        {
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

            foreach (IBindableUI b in BindableUIs)
            {
                b.CleanUp();
            }
        }

        #region canvas events
        private void OnRightClick(ContextClickEvent evt)
        {
            OnNodeRightClicked?.Invoke(this, evt);
        }

        private void OnClick(ClickEvent evt)
        {
            OnNodeClicked?.Invoke(this, evt);
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

            OnNodeMoved?.Invoke(this);
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
