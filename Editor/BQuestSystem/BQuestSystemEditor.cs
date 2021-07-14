using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Bloodthirst.Editor;
using System.Collections.Generic;
using System;
using UnityEditor.UIElements;
using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using Bloodthirst.Core.TreeList;
using System.Linq;
using UnityEditor.Callbacks;

namespace Bloodthirst.System.Quest.Editor
{
    public class BQuestSystemEditor : EditorWindow, INodeEditor
    {
        /// <summary>
        /// UXML path
        /// </summary>
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/BQuestSystemEditor.uxml";

        /// <summary>
        /// USS path
        /// </summary>
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BQuestSystem/BQuestSystemEditor.uss";
        private const int MAX_NODE_ID = 100;
        private float zoom = 1;


        [MenuItem("Bloodthirst Tools/BQuestSystemEditor")]
        public static void OpenWindow()
        {
            BQuestSystemEditor wnd = GetWindow<BQuestSystemEditor>();
            wnd.titleContent = new GUIContent("BQuestSystemEditor");
        }

        [DidReloadScripts()]
        public static void OnScriptReload()
        {
            if (EditorWindow.HasOpenInstances<BQuestSystemEditor>())
                return;

            BQuestSystemEditor win = GetWindow<BQuestSystemEditor>();

            if (!win.IsInitialized)
                return;

            win.RefreshObjectPickers(true);

        }

        #region global params
        public bool IsInitialized { get; set; }
        public float ZoomSensitivity { get; set; } = 0.1f;
        public bool InvertZoom { get; set; } = true;
        public Vector2 ZoomMinMax { get; set; }
        public Type NodeBaseType { get; set; }
        public float Zoom
        {
            get => zoom;
            set
            {
                float old = zoom;
                zoom = Mathf.Clamp(value, ZoomMinMax.x, ZoomMinMax.y);
                OnZoomChanged?.Invoke(old, zoom);
            }
        }

        public Vector2 PanningOffset { get; set; }

        #endregion

        #region UI elements

        private VisualElement Root => rootVisualElement;
        private VisualElement NodeTypePickerContainer => Root.Q<VisualElement>(nameof(NodeTypePickerContainer));
        private PopupField<Type> NodeTypePicker { get; set; }
        public VisualElement Canvas => Root.Q<VisualElement>(nameof(Canvas));
        public VisualElement Container => Root.Q<VisualElement>(nameof(Container));
        public Button SaveBtn => Root.Q<Button>(nameof(SaveBtn));
        public Button LoadBtn => Root.Q<Button>(nameof(LoadBtn));
        public ObjectField NodeTreeDataPicker => Root.Q<ObjectField>(nameof(NodeTreeDataPicker));
        public ObjectField NodeBehaviourPicker => Root.Q<ObjectField>(nameof(NodeBehaviourPicker));

        #endregion

        #region nodes state

        private List<int> CurrentNodeIDs { get; set; }
        private HashSet<NodeBaseElement> SelectedNodes { get; set; }
        private List<NodeBaseElement> AllNodes { get; set; }
        private List<LinkElement> AllLinks { get; set; }

        private BNodeTreeBehaviourBase selectedTreeBehaviour;
        private BNodeTreeBehaviourBase SelectedTreeBehaviour
        {
            get => selectedTreeBehaviour;
            set
            {
                // refresh ui first
                NodeBehaviourPicker.value = value;

                // if behaviour didn't change
                if (selectedTreeBehaviour == value)
                    return;

                selectedTreeBehaviour = value;

                if (selectedTreeBehaviour != null)
                {
                    SelectedTreeData = selectedTreeBehaviour.TreeData;
                }

                OnBehaviourSelectionChanged?.Invoke(selectedTreeBehaviour);
            }
        }

        private BNodeTreeBehaviourBase SelectedTreeBehaviourForce
        {
            set
            {
                // refresh ui first
                NodeBehaviourPicker.value = value;

                selectedTreeBehaviour = value;

                // change the selected data

                if (selectedTreeBehaviour != null)
                {
                    SelectedTreeDataForce = selectedTreeBehaviour.TreeData;
                }

                OnBehaviourSelectionChanged?.Invoke(selectedTreeBehaviour);
            }
        }

        private NodeTreeData selectedNodeData;
        private NodeTreeData SelectedTreeData
        {
            get => selectedNodeData;
            set
            {
                // refresh ui first
                NodeTreeDataPicker.value = value;

                // if behaviour didn't change
                if (selectedNodeData == value)
                {
                    return;
                }

                SelectedTreeDataForce = value;

            }
        }

        private NodeTreeData SelectedTreeDataForce
        {
            set
            {
                // refresh ui first
                NodeTreeDataPicker.value = value;

                selectedNodeData = value;

                if (selectedNodeData != null)
                {
                    Load(selectedNodeData);
                }

                OnDataSelectionChanged?.Invoke(selectedNodeData);
            }
        }

        Type INodeEditor.NodeBaseType => NodeBaseType;
        HashSet<NodeBaseElement> INodeEditor.SelectedNodes => SelectedNodes;
        List<NodeBaseElement> INodeEditor.AllNodes => AllNodes;
        List<LinkElement> INodeEditor.AllLinks => AllLinks;
        List<int> INodeEditor.CurrentNodeIDs => CurrentNodeIDs;

        #endregion

        public event Action<float, float> OnZoomChanged;

        public event Action<KeyDownEvent> OnWindowKeyPressed;

        public event Action<ClickEvent> OnCanvasMouseClick;

        public event Action<ContextClickEvent> OnCanvasMouseContextClick;

        public event Action<NodeBaseElement, ClickEvent> OnNodeMouseClick;

        public event Action<NodeBaseElement> OnNodeStartResize;
        
        public event Action<NodeBaseElement> OnNodeEndResize;

        public event Action<NodeBaseElement> OnNodeAddInput;

        public event Action<NodeBaseElement> OnNodeAddOutput;

        public event Action<PortBaseElement> OnPortToggleInfo;

        public event Action<NodeBaseElement, ContextClickEvent> OnNodeMouseContextClick;

        public event Action<PortBaseElement, ContextClickEvent> OnPortMouseContextClick;

        public event Action<BNodeTreeBehaviourBase> OnBehaviourSelectionChanged;

        public event Action<NodeTreeData> OnDataSelectionChanged;

        public event Action<WheelEvent> OnCanvasScrollWheel;

        public event Action<MouseDownEvent> OnCanvasMouseDown;

        public event Action<MouseLeaveEvent> OnCanvasMouseLeave;

        public event Action<MouseUpEvent> OnCanvasMouseUp;

        public event Action<MouseMoveEvent> OnCanvasMouseMove;
        private List<INodeEditorAction> CanvasActions { get; set; }

        private void OnDisable()
        {
            if (!IsInitialized)
                return;

            foreach (INodeEditorAction a in CanvasActions)
            {
                a.OnDisable();
                a.NodeEditor = null;
            }

            SaveBtn.clickable.clicked -= HandleSaveClicked;
            LoadBtn.clickable.clicked -= HandleLoadClicked;
            Root.RegisterCallback<KeyDownEvent>(OnKeyPress);

            //// Register container events
            // scroll
            Container.UnregisterCallback<ClickEvent>(OnMouseClick);
            Container.UnregisterCallback<ContextClickEvent>(OnRightClick);
            Container.UnregisterCallback<WheelEvent>(OnScrollWheel);
            Container.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            Container.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
            Container.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            Container.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            NodeTypePicker.UnregisterValueChangedCallback(HandleNodeTypeChanged);


            EditorApplication.update -= Update;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            Selection.selectionChanged -= HandleObjectSelection;

            IsInitialized = false;

        }

        private void HandleObjectSelection()
        {
            RefreshObjectPickers();
        }

        private void ClearCanvas()
        {
            for (int i = AllNodes.Count - 1; i >= 0; i--)
            {
                NodeBaseElement curr = AllNodes[i];
                RemoveNode(curr);
            }
        }


        private void WaitForNodeConstruction()
        {
            foreach (NodeBaseElement n in AllNodes)
            {
                if (float.IsNaN(n.VisualElement.contentRect.width))
                    return;
            }

            EditorApplication.update -= WaitForNodeConstruction;

            foreach (LinkElement l in AllLinks)
            {
                l.Refresh();
            }
        }

        private void Update()
        {
            if (Canvas == null)
                return;


            Canvas.transform.scale = Vector3.one * Zoom;
            Canvas.transform.position = PanningOffset;
        }

        private void SetupCanvasActions()
        {
            CanvasActions = new List<INodeEditorAction>()
            {
                // nodes
                new AddNodeAction(),
                new RemoveNodeAction(),
                new ResizeNodeAction(),
                new AddPortInputAction(),
                new AddPortOutputAction(),
                new TogglePortInfoAction(),

                // selection
                new SelectNodeAction(),
                new DeselectNodeAction(),

                // link
                new LinkNodesAction(),
                
                // view control
                new PanCanvasAction(),
                new ZoomCanvasAction(),

                // global
                new BuildNodeTreeAction(),

                // behaviour
                new ActiveNodeBehaviourAction()

            };

            foreach (INodeEditorAction a in CanvasActions)
            {
                a.NodeEditor = this;
            }
        }

        public void CreateGUI()
        {

            SetupCanvasActions();

            this.SetAntiAliasing(4);

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;

            Selection.selectionChanged -= HandleObjectSelection;
            Selection.selectionChanged += HandleObjectSelection;


            AllNodes = new List<NodeBaseElement>();
            AllLinks = new List<LinkElement>();
            SelectedNodes = new HashSet<NodeBaseElement>();
            CurrentNodeIDs = new List<int>();
            ZoomMinMax = new Vector2(0.1f, 10);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            VisualElement uxml = visualTree.Instantiate();

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // makes the container stretech to the rect of the window
            uxml.AddToClassList("w-100");
            uxml.AddToClassList("h-100");

            Root.Add(uxml);
            Root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
            Root.styleSheets.Add(customUss);

            Intialize();

            foreach (INodeEditorAction a in CanvasActions)
            {
                a.OnEnable();
            }

            // if data is already loaded
            // don't init with a node
            if (SelectedTreeData == null)
            {
                DefaultNode firstNode = new DefaultNode();

                AddNode(firstNode, Vector3.zero);
            }

            IsInitialized = true;
        }

        private void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode)
            {
                RefreshObjectPickers(true);
            }
        }

        private void RefreshObjectPickers(bool force = false)
        {
            BNodeTreeBehaviourBase tree = null;
            NodeTreeData data = null;

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                UnityEngine.Object curr = Selection.objects[i];

                if (curr is GameObject g)
                {
                    tree = g.GetComponent<BNodeTreeBehaviourBase>();
                    break;
                }

                if (curr is NodeTreeData d)
                {
                    data = d;
                    break;
                }
            }

            // if behaviour is selected
            // we set it and exit
            if (!force)
            {
                SelectedTreeBehaviour = tree;
            }
            else
            {
                SelectedTreeBehaviourForce = tree;
            }

            if (tree != null)
            {
                return;
            }

            // if no behaviour is selected
            // go to the data

            if (!force)
            {
                SelectedTreeData = data;
            }
            else
            {
                SelectedTreeDataForce = data;
            }
        }

        private void Intialize()
        {
            List<Type> validNodeTypes = TypeUtils.AllTypes
                .Where(t => t.IsAbstract)
                .Where(t => t != typeof(INodeType))
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(INodeType)))
                .ToList();

            NodeTypePicker = new PopupField<Type>("Node base type", validNodeTypes, 0, TypeUtils.GetNiceName, TypeUtils.GetNiceName);
            NodeTypePickerContainer.Add(NodeTypePicker);
            NodeBaseType = validNodeTypes[0];

            // data pickers
            NodeBehaviourPicker.objectType = typeof(BNodeTreeBehaviourBase);
            NodeTreeDataPicker.objectType = typeof(NodeTreeData);

            // init object selection
            RefreshObjectPickers(true);

            /*
            NodeBehaviourPicker.SetEnabled(false);
            NodeTreeDataPicker.SetEnabled(false);
            */

            // node type
            NodeTypePicker.RegisterValueChangedCallback(HandleNodeTypeChanged);

            SaveBtn.clickable.clicked += HandleSaveClicked;
            LoadBtn.clickable.clicked += HandleLoadClicked;
            Root.RegisterCallback<KeyDownEvent>(OnKeyPress);

            //// Register container events
            // scroll
            Container.RegisterCallback<ClickEvent>(OnMouseClick);
            Container.RegisterCallback<ContextClickEvent>(OnRightClick);
            Container.RegisterCallback<WheelEvent>(OnScrollWheel);
            Container.RegisterCallback<MouseDownEvent>(OnMouseDown);
            Container.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            Container.RegisterCallback<MouseUpEvent>(OnMouseUp);
            Container.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }

        private void HandleNodeTypeChanged(ChangeEvent<Type> evt)
        {
            NodeBaseType = evt.newValue;
            NodeTypePicker.SetValueWithoutNotify(evt.newValue);
        }


        public void AddNode(INodeType n, Vector2 nodePosition, Vector2? size = null, bool worldSpace = true)
        {
            // if no ID is assigned
            // then create one
            if (n.NodeID == -1)
            {
                int generatedID = -1;

                do
                {
                    generatedID = UnityEngine.Random.Range(0, MAX_NODE_ID);
                }
                while (CurrentNodeIDs.Contains(generatedID));

                n.NodeID = generatedID;
            }

            NodeBaseElement node = new NodeBaseElement(n);


            // add nodes to list
            AllNodes.Add(node);
            CurrentNodeIDs.Add(n.NodeID);

            node.OnNodeClicked -= OnNodeClicked;
            node.OnNodeClicked += OnNodeClicked;

            node.OnNodeAddOutput -= HandleNodeAddOutput;
            node.OnNodeAddOutput += HandleNodeAddOutput;

            node.OnNodeAddInput -= HandleNodeAddInput;
            node.OnNodeAddInput += HandleNodeAddInput;

            node.OnNodeStartResize -= OnNodeStartResized;
            node.OnNodeStartResize += OnNodeStartResized;

            node.OnNodeEndResize -= OnNodeEndResized;
            node.OnNodeEndResize += OnNodeEndResized;


            node.OnNodeRightClicked -= OnNodeRightClicked;
            node.OnNodeRightClicked += OnNodeRightClicked;


            // add to ui

            node.BeforeAddToCanvas();

            Canvas.Add(node.VisualElement);
            node.VisualElement.transform.position = worldSpace ? WorldToCanvas(nodePosition) : (Vector2)nodePosition;

            node.AfterAddToCanvas();

            // subscribe to ports events

            for (int i = 0; i < node.InputsConst.Count; i++)
            {
                node.InputsConst[i].OnPortRightClicked -= HandlePortRightClick;
                node.InputsConst[i].OnPortRightClicked += HandlePortRightClick;

                node.InputsConst[i].OnPortToggleInfoDialog -= HandlePortToggle;
                node.InputsConst[i].OnPortToggleInfoDialog += HandlePortToggle;
            }

            for (int i = 0; i < node.OutputsConst.Count; i++)
            {
                node.OutputsConst[i].OnPortRightClicked -= HandlePortRightClick;
                node.OutputsConst[i].OnPortRightClicked += HandlePortRightClick;

                node.OutputsConst[i].OnPortToggleInfoDialog -= HandlePortToggle;
                node.OutputsConst[i].OnPortToggleInfoDialog += HandlePortToggle;
            }

            if (size.HasValue)
            {
                node.NodeSize = size.Value;
            }
        }

        private void HandleNodeAddInput(NodeBaseElement node)
        {
            OnNodeAddInput?.Invoke(node);
        }

        private void HandleNodeAddOutput(NodeBaseElement node)
        {
            OnNodeAddOutput?.Invoke(node);
        }

        private void OnNodeEndResized(NodeBaseElement node)
        {
            OnNodeEndResize?.Invoke(node);
        }

        public void OnNodeStartResized(NodeBaseElement node)
        {
            OnNodeStartResize?.Invoke(node);
        }

        public LinkElement AddLink(PortBaseElement from, PortBaseElement to)
        {
            // link type
            LinkDefault linkType = new LinkDefault() { From = from.PortType, To = to.PortType };

            // ui
            LinkElement link = new LinkElement(linkType, from, to);

            Canvas.Add(link.VisualElement);
            AllLinks.Add(link);

            // links should always be behind the nodes
            link.VisualElement.SendToBack();

            link.AfterAddToCanvas();

            return link;

        }

        public void RemoveNode(NodeBaseElement node)
        {
            // instance remove
            AllNodes.Remove(node);
            SelectedNodes.Remove(node);
            CurrentNodeIDs.Remove(node.NodeType.NodeID);

            // ui remove
            node.OnNodeClicked -= OnNodeClicked;
            node.OnNodeStartResize -= OnNodeStartResized;
            node.OnNodeEndResize -= OnNodeEndResized;
            node.OnNodeRightClicked -= OnNodeRightClicked;
            node.OnNodeAddInput -= HandleNodeAddInput;
            node.OnNodeAddOutput -= HandleNodeAddOutput;

            node.BeforeRemoveFromCanvas();
            Canvas.Remove(node.VisualElement);

            // clear links
            for (int i = 0; i < node.InputsConst.Count; i++)
            {
                node.InputsConst[i].OnPortRightClicked -= HandlePortRightClick;
                node.InputsConst[i].OnPortToggleInfoDialog -= HandlePortToggle;
            }

            for (int i = 0; i < node.OutputsConst.Count; i++)
            {
                node.OutputsConst[i].OnPortRightClicked -= HandlePortRightClick;
                node.OutputsConst[i].OnPortToggleInfoDialog -= HandlePortToggle;
            }

            for (int i = AllLinks.Count - 1; i >= 0; i--)
            {
                LinkElement curr = AllLinks[i];

                if (curr.From.ParentNode != node && curr.To.ParentNode != node)
                    continue;

                curr.From.Link = null;
                curr.To.Link = null;

                AllLinks.RemoveAt(i);
                curr.BeforeRemoveFromCanvas();
                Canvas.Remove(curr.VisualElement);
            }

            node.AfterRemoveFromCanvs();
        }

        #region save & load


        [OnOpenAsset]
        public static bool Open(int instanceID, int line)
        {
            UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);

            if (!(obj is NodeTreeData data))
                return false;

            if (obj == null)
                return false;

            GetWindow<BQuestSystemEditor>().SelectedTreeData = data;

            return true;
        }
        public bool Save()
        {
            string savePath = EditorUtility.SaveFilePanel("Save tree data", "Assets", "TreeNodeData", "asset");

            if (string.IsNullOrEmpty(savePath))
                return false;

            NodeTreeData treeData = GetTreeData();

            string projectPath = Application.dataPath;



            if (!savePath.StartsWith(projectPath))
                return false;

            savePath = FileUtil.GetProjectRelativePath(savePath);

            Debug.Log($"Tree Node Data Saved at : {savePath}");

            NodeTreeData existingAsset = AssetDatabase.LoadAssetAtPath<NodeTreeData>(savePath);

            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(treeData, savePath);
            }
            else
            {
                EditorUtility.CopySerializedIfDifferent(treeData, existingAsset);
                AssetDatabase.SaveAssets();
            }

            return true;
        }

        private NodeTreeData GetTreeData()
        {
            NodeTreeData treeData = CreateInstance<NodeTreeData>();
            treeData.NodeBaseType = NodeBaseType;
            List<NodeData> nodeData = treeData.Nodes;
            List<LinkData> linkData = treeData.Links;

            for (int i = 0; i < AllNodes.Count; i++)
            {
                NodeBaseElement curr = AllNodes[i];

                NodeData n = new NodeData()
                {
                    NodeType = curr.NodeType,
                    Position = curr.VisualElement.localBound.position,
                    Size = curr.NodeSize
                };

                nodeData.Add(n);
            }

            for (int i = 0; i < AllLinks.Count; i++)
            {
                LinkElement curr = AllLinks[i];
                int idFrom = curr.From.ParentNode.NodeType.NodeID;
                int idTo = curr.To.ParentNode.NodeType.NodeID;
                int indexPortFrom = curr.From.ParentNode.OutputsConst.IndexOf(curr.From);
                int indexPortTo = curr.To.ParentNode.InputsConst.IndexOf(curr.To);
                LinkData l = new LinkData()
                {
                    From = idFrom,
                    To = idTo,
                    FromPort = indexPortFrom,
                    ToPort = indexPortTo
                };

                linkData.Add(l);
            }

            return treeData;
        }

        public void Load(NodeTreeData data)
        {
            ClearCanvas();

            NodeTypePicker.value = data.NodeBaseType;

            foreach (NodeData n in data.Nodes)
            {
                AddNode(n.NodeType, n.Position , n.Size , false);
            }

            foreach (LinkData l in data.Links)
            {
                NodeBaseElement fromNode = AllNodes.FirstOrDefault(n => n.NodeType.NodeID == l.From);
                NodeBaseElement toNode = AllNodes.FirstOrDefault(n => n.NodeType.NodeID == l.To);
                PortBaseElement fromPort = fromNode.OutputsConst[l.FromPort];
                PortBaseElement toPort = toNode.InputsConst[l.ToPort];
                AddLink(fromPort, toPort);
            }

            EditorApplication.update += WaitForNodeConstruction;
        }
        #endregion


        #region utils

        public Vector2 WorldToCanvas(Vector2 pos)
        {
            return Canvas.WorldToLocal(pos);
        }

        public Vector2 WorldToContainer(Vector2 pos)
        {
            return Container.WorldToLocal(pos);
        }

        /// <summary>
        /// Is mouse inside the rect of a visual element ?
        /// </summary>
        /// <param name="pos">The world space position of the mouse (evt.position)</param>
        /// <returns></returns>
        public bool IsInsideNode(Vector2 pos)
        {
            foreach (NodeBaseElement n in AllNodes)
            {
                VisualElement curr = n.VisualElement;

                if (curr.worldBound.Contains(pos))
                    return true;
            }

            return false;
        }

        #endregion

        #region mouse & window events

        private void HandleLoadClicked()
        {
            string loadAssetPath = EditorUtility.OpenFilePanel("Open tree data", "Assets", "asset");

            string projectPath = Application.dataPath;

            if (!loadAssetPath.StartsWith(projectPath))
                return;

            loadAssetPath = "Assets" + loadAssetPath.Substring(projectPath.Length);


            NodeTreeData loadAsset = AssetDatabase.LoadAssetAtPath<NodeTreeData>(loadAssetPath);

            if (loadAsset == null)
                return;

            Load(loadAsset);
        }

        private void HandleSaveClicked()
        {
            Save();
        }

        private void OnNodeClicked(NodeBaseElement node, ClickEvent evt)
        {
            OnNodeMouseClick?.Invoke(node, evt);
        }

        private void HandlePortToggle(PortBaseElement portBaseElement)
        {
            OnPortToggleInfo?.Invoke(portBaseElement);
        }

        private void OnKeyPress(KeyDownEvent evt)
        {
            OnWindowKeyPressed?.Invoke(evt);
        }

        private void HandlePortRightClick(PortBaseElement port, ContextClickEvent evt)
        {
            OnPortMouseContextClick?.Invoke(port, evt);
        }

        private void OnNodeRightClicked(NodeBaseElement node, ContextClickEvent evt)
        {
            OnNodeMouseContextClick?.Invoke(node, evt);
        }
        private void OnRightClick(ContextClickEvent evt)
        {
            if (IsInsideNode(evt.mousePosition))
                return;

            OnCanvasMouseContextClick?.Invoke(evt);
        }

        private void OnMouseClick(ClickEvent evt)
        {
            if (IsInsideNode(evt.position))
                return;

            OnCanvasMouseClick?.Invoke(evt);
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            OnCanvasMouseMove?.Invoke(evt);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            OnCanvasMouseUp?.Invoke(evt);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            OnCanvasMouseDown?.Invoke(evt);
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            OnCanvasMouseLeave?.Invoke(evt);
        }

        private void OnScrollWheel(WheelEvent evt)
        {
            OnCanvasScrollWheel?.Invoke(evt);
        }


        #endregion
    }
}