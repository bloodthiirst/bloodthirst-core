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
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/Node/NodeBaseElement.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/Node/NodeBaseElement.uss";
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
        public event Action<NodeBaseElement> OnNodeAddInput;
        public event Action<NodeBaseElement> OnNodeAddOutput;

        #endregion

        #region ui elements
        private VisualElement NodeRoot { get; set; }
        private VisualElement NodeContent => NodeRoot.Q<VisualElement>(nameof(NodeContent));
        private VisualElement NodeHeader => NodeRoot.Q<VisualElement>(nameof(NodeHeader));
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
        public List<PortBaseElement> InputsConst { get; }
        public List<PortBaseElement> InputsVariable { get; }
        public List<PortBaseElement> OutputsConst { get; }
        public List<PortBaseElement> OutputsVariable { get; }
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
                float minWidth = NodeContent.localBound.width;
                float minHeight = NodeContent.localBound.height;

                float w = Mathf.Max(value.x, minWidth);
                float h = Mathf.Max(value.y, minHeight);

                NodeRoot.style.width = new Length(w, LengthUnit.Pixel);
                NodeRoot.style.height = new Length(h, LengthUnit.Pixel);
                NodeRoot.MarkDirtyRepaint();

                OnNodeResized?.Invoke(this);
            }
        }

        public NodeBaseElement(INodeType nodeType)
        {
            InputsConst = new List<PortBaseElement>();
            InputsVariable = new List<PortBaseElement>();

            OutputsConst = new List<PortBaseElement>();
            OutputsVariable = new List<PortBaseElement>();

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

            SetupFields();

            FixLabels();
        }

        private void FixLabels()
        {
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

        private IEnumerable<MemberInfo> GetMembers()
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

        private List<MemberInfo> ValidMembers()
        {
            IEnumerable<MemberInfo> members = GetMembers()
                .Where(m => m.Name != "NodeID")
                .Where(m => !m.Name.EndsWith("__BackingField"));


            return members.ToList();
        }

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

        /// <summary>
        /// Get all input and output ports
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PortBaseElement> AllPorts()
        {
            foreach (PortBaseElement n in InputsConst)
            {
                yield return n;
            }

            foreach (PortBaseElement n in InputsVariable)
            {
                yield return n;
            }

            foreach (PortBaseElement n in OutputsConst)
            {
                yield return n;
            }

            foreach (PortBaseElement n in OutputsVariable)
            {
                yield return n;
            }
        }

        #region add input
        private void AddConstInputPort(IPortType curr)
        {
            PortBaseElement input = new PortBaseElement(this, curr);
            InputPortsContainer.Add(input.VisualElement);
            InputsConst.Add(input);
        } 
        private void AddVariableInputPort(IPortType curr)
        {
            PortBaseElement input = new PortBaseElement(this, curr);
            InputPortsContainer.Add(input.VisualElement);
            InputsConst.Add(input);
        }
        #endregion

        #region add output
        private void AddConstOutputPort(IPortType curr)
        {
            PortBaseElement output = new PortBaseElement(this, curr);
            OutputPortsContainer.Add(output.VisualElement);
            OutputsConst.Add(output);
        }

        private void AddVariableOutputPort(IPortType curr)
        {
            PortBaseElement output = new PortBaseElement(this, curr);
            OutputPortsContainer.Add(output.VisualElement);
            OutputsConst.Add(output);
        }
        #endregion

        #region remove output
        private void RemoveConstOutputAt(int i)
        {
            PortBaseElement curr = OutputsConst[i];
            OutputPortsContainer.RemoveAt(i);
            OutputsConst.RemoveAt(i);
        }

        private void RemoveVariableOutputAt(int i)
        {
            PortBaseElement curr = OutputsConst[i];
            OutputPortsContainer.RemoveAt(i);
            OutputsConst.RemoveAt(i);
        }
        #endregion

        #region remove input
        private void RemoveConstInputAt(int i)
        {
            PortBaseElement curr = InputsConst[i];
            InputsConst.RemoveAt(i);

            InputPortsContainer.Remove(curr.VisualElement);
        }

        private void RemoveVariableInputAt(int i)
        {
            PortBaseElement curr = InputsVariable[i];
            InputsVariable.RemoveAt(i);

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
            // output
            for (int i = 0; i < NodeType.OutputPortsConst.Count; i++)
            {
                IPortType curr = NodeType.OutputPortsConst[i];
                AddConstOutputPort(curr);
            }

            for (int i = 0; i < NodeType.OutputPortsVariable.Count; i++)
            {
                IPortType curr = NodeType.OutputPortsVariable[i];
                AddVariableOutputPort(curr);
            }

            // input
            for (int i = 0; i < NodeType.InputPortsConst.Count; i++)
            {
                IPortType curr = NodeType.InputPortsConst[i];
                AddConstInputPort(curr);
            }
            for (int i = 0; i < NodeType.InputPortsVariable.Count; i++)
            {
                IPortType curr = NodeType.InputPortsVariable[i];
                AddVariableInputPort(curr);
            }
        }

        private void ClearAllPorts()
        {
            // outputs
            for (int i = NodeType.OutputPortsConst.Count - 1; i >= 0; i--)
            {
                RemoveConstOutputAt(i);
            }
            for (int i = NodeType.OutputPortsVariable.Count - 1; i >= 0; i--)
            {
                RemoveVariableOutputAt(i);
            }

            // inputs
            for (int i = NodeType.InputPortsConst.Count - 1; i >= 0; i--)
            {
                RemoveConstInputAt(i);
            }
            for (int i = NodeType.InputPortsVariable.Count - 1; i >= 0; i--)
            {
                RemoveVariableInputAt(i);
            }
        }

        public void BeforeAddToCanvas()
        {
            AddAllPorts();
        }

        public void AfterAddToCanvas()
        {
            // adding
            AddInput.clickable.clicked -= HandleAddInput;
            AddInput.clickable.clicked += HandleAddInput;
            
            AddOutput.clickable.clicked -= HandleAddOutput;
            AddOutput.clickable.clicked += HandleAddOutput;

            // moving
            NodeHeader.RegisterCallback<MouseDownEvent>(OnMouseDown);
            NodeHeader.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            NodeHeader.RegisterCallback<MouseUpEvent>(OnMouseUp);
            NodeHeader.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            NodeRoot.RegisterCallback<ClickEvent>(OnClick);
            NodeRoot.RegisterCallback<ContextClickEvent>(OnRightClick);

            //resizing
            NodeResize.RegisterCallback<MouseEnterEvent>(OnResizeEnter);
            NodeResize.RegisterCallback<MouseLeaveEvent>(OnResizeLeave);
            NodeResize.RegisterCallback<MouseDownEvent>(OnResizeDown);
            NodeResize.RegisterCallback<MouseUpEvent>(OnResizeUp);

            foreach (PortBaseElement p in AllPorts())
            {
                p.AfterAddToCanvas();
            }
        }

        private void HandleAddOutput()
        {
            OnNodeAddInput?.Invoke(this);
        }

        private void HandleAddInput()
        {
            OnNodeAddOutput?.Invoke(this);
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

            foreach (PortBaseElement p in AllPorts())
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
