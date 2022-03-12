using Bloodthirst.BEventSystem;
using Bloodthirst.Core.TreeList;
using Bloodthirst.Runtime.BNodeTree;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
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
        BEventSystem<BNodeTreeEventBase> BEventSystem { get; set; }
        Vector2 CanvasSize { get; }

        LinkElement AddLink(PortBaseElement start, PortBaseElement end);
        void AddNode(INodeType node, Vector2 worldMousePos , Vector2? size , bool worldSpace = true);
        bool IsInsideNode(Vector2 pos);
        void RemoveNode(NodeBaseElement node);
        Vector2 WorldToCanvas(Vector2 pos);
        Vector2 WorldToContainer(Vector2 pos);
    }
}
