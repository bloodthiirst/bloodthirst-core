using Bloodthirst.Core.TreeList;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public interface INodeEditor
    {
        List<int> CurrentNodeIDs { get; }
        List<NodeBaseElement> AllNodes { get; }
        List<LinkElement> AllLinks { get; }
        HashSet<NodeBaseElement> SelectedNodes { get; }
        Vector2 PanningOffset { get; set; }
        bool InvertZoom { get; set; }
        float ZoomSensitivity { get; set; }
        float Zoom { get; set; }
        Type NodeBaseType { get; }

        event Action<float,float> OnZoomChanged;

        event Action<ClickEvent> OnCanvasMouseClick;

        event Action<ContextClickEvent> OnCanvasMouseContextClick;

        event Action<NodeBaseElement, ClickEvent> OnNodeMouseClick;

        event Action<NodeBaseElement , ContextClickEvent> OnNodeMouseContextClick;

        event Action<WheelEvent> OnCanvasScrollWheel;

        event Action<MouseDownEvent> OnCanvasMouseDown;

        event Action<MouseLeaveEvent> OnCanvasMouseLeave;

        event Action<MouseUpEvent> OnCanvasMouseUp;

        event Action<MouseMoveEvent> OnCanvasMouseMove;
        event Action<KeyDownEvent> OnWindowKeyPressed;
        event Action<PortBaseElement, ContextClickEvent> OnPortMouseContextClick;
        event Action<BNodeTreeBehaviour> OnBehaviourSelectionChanged;
        event Action<NodeTreeData> OnDataSelectionChanged;

        event Action<NodeBaseElement> OnNodeStartResize;
        event Action<NodeBaseElement> OnNodeEndResize;
        event Action<NodeBaseElement> OnNodeAddInput;
        event Action<NodeBaseElement> OnNodeAddOutput;

        LinkElement AddLink(PortBaseElement start, PortBaseElement end);
        void AddNode(INodeType node, Vector2 worldMousePos , Vector2? size , bool worldSpace = true);
        bool IsInsideNode(Vector2 pos);
        void RemoveNode(NodeBaseElement node);
        Vector2 WorldToCanvas(Vector2 pos);
        Vector2 WorldToContainer(Vector2 pos);
    }
}
