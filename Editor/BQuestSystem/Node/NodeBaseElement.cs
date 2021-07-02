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
        private const string NODE_SELECTED_IN_CANVAS_USS_CLASS = "node-selected";
        private const string NODE_SELECTED_ACTIVE_USS_CLASS = "active";
        #endregion

        #region events
        public event Action<NodeBaseElement, ClickEvent> OnNodeClicked;
        public event Action<NodeBaseElement, ContextClickEvent> OnNodeRightClicked;
        public event Action<NodeBaseElement, ClickEvent> OnNodeRequestRemove;
        public event Action<NodeBaseElement> OnNodeMoved;
        public event Action<NodeBaseElement> OnNodeStartResize;
        public event Action<NodeBaseElement> OnNodeEndResize;

        #endregion

        #region ui elements
        private VisualElement NodeRoot { get; set; }
        private VisualElement NodeContent => NodeRoot.Q<VisualElement>(nameof(NodeContent));
        private VisualElement NodeHeader => NodeRoot.Q<VisualElement>(nameof(NodeHeader));
        private VisualElement InputPortsContainer => NodeRoot.Q<VisualElement>(nameof(InputPortsContainer));
        private VisualElement OutputPortsContainer => NodeRoot.Q<VisualElement>(nameof(OutputPortsContainer));
        private VisualElement FieldsContainer => NodeRoot.Q<VisualElement>(nameof(FieldsContainer));
        private VisualElement NodeActive => NodeRoot.Q<VisualElement>(nameof(NodeActive));
        private VisualElement NodeResize => NodeRoot.Q<VisualElement>(nameof(NodeResize));
        public VisualElement VisualElement => NodeRoot;
        public List<PortBaseElement> Inputs { get; }
        public List<PortBaseElement> Outputs { get; }
        public List<IBindableUI> BindableUIs { get; }
        #endregion
        public bool IsInsideResize { get; set; }
        public bool CanResize { get; set; }
        public bool CanDrag { get; set; }
        public bool IsNodeSelected { get; set; }
        public bool IsNodeActive { get; set; }
        public INodeType NodeType { get; }

        private Vector2 LastMousePressPosition;

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
            }
        }

        public NodeBaseElement(INodeType nodeType)
        {
            Inputs = new List<PortBaseElement>();
            Outputs = new List<PortBaseElement>();
            BindableUIs = new List<IBindableUI>();

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer templateContainer = visualTree.Instantiate();

            NodeRoot = templateContainer.Q<VisualElement>(nameof(NodeRoot));

            NodeRoot.styleSheets.Add(customUss);

            NodeType = nodeType;

            SetupFields();

            FixLabels();
        }

        private void FixLabels()
        {
            // max label length
            int maxLabelLength = BindableUIs.Max(l => l.MemberInfo.Name.Length);

            // ratio picked
            float fontRatio = 70 / 11f;

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
            foreach (PortBaseElement n in Inputs)
            {
                yield return n;
            }

            foreach (PortBaseElement n in Outputs)
            {
                yield return n;
            }
        }

        private void AddInputPort(IPortType curr)
        {
            PortBaseElement input = new PortBaseElement(this, curr);
            InputPortsContainer.Add(input.VisualElement);
            Inputs.Add(input);
        }

        private void AddOutputPort(IPortType curr)
        {
            PortBaseElement output = new PortBaseElement(this, curr);
            OutputPortsContainer.Add(output.VisualElement);
            Outputs.Add(output);
        }

        private void RemoveOutputAt(int i)
        {
            PortBaseElement curr = Outputs[i];
            OutputPortsContainer.RemoveAt(i);
            Outputs.RemoveAt(i);
        }

        private void RemoveInputAt(int i)
        {
            PortBaseElement curr = Inputs[i];
            InputPortsContainer.RemoveAt(i);
            Inputs.RemoveAt(i);
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
            for (int i = 0; i < NodeType.OutputPorts.Count; i++)
            {
                IPortType curr = NodeType.OutputPorts[i];
                AddOutputPort(curr);
            }

            for (int i = 0; i < NodeType.InputPorts.Count; i++)
            {
                IPortType curr = NodeType.InputPorts[i];
                AddInputPort(curr);
            }
        }

        private void ClearAllPorts()
        {
            for (int i = NodeType.OutputPorts.Count - 1; i >= 0; i--)
            {
                RemoveOutputAt(i);
            }

            for (int i = NodeType.InputPorts.Count - 1; i >= 0; i--)
            {
                RemoveInputAt(i);
            }
        }

        public void BeforeAddToCanvas()
        {
            AddAllPorts();
        }

        public void AfterAddToCanvas()
        {
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
            NodeRoot.AddToClassList(NODE_SELECTED_IN_CANVAS_USS_CLASS);
        }

        public void DeselectInCanvas()
        {
            IsNodeSelected = false;
            NodeRoot.RemoveFromClassList(NODE_SELECTED_IN_CANVAS_USS_CLASS);
        }

        public void SelectActive()
        {
            IsNodeActive = true;
            NodeActive.AddToClassList(NODE_SELECTED_ACTIVE_USS_CLASS);
        }

        public void DeselectActive()
        {
            IsNodeActive = false;
            NodeActive.RemoveFromClassList(NODE_SELECTED_ACTIVE_USS_CLASS);
        }
    }
}
