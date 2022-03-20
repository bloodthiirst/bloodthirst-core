using Bloodthirst.Runtime.BNodeTree;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public class LinkElement
    {
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Link/LinkElement.uss";

        public INodeEditor NodeEditor { get; }
        public ILinkType LinkType { get; set; }    
        public PortBaseElement From { get; private set; }
        public PortBaseElement To { get; private set; }
        private UILinelement Root { get; set; }
        public VisualElement VisualElement => Root;

        public LinkElement( INodeEditor nodeEditor, ILinkType link, PortBaseElement from, PortBaseElement to)
        {
            From = from;
            To = to;
            NodeEditor = nodeEditor;
            LinkType = link;
            
            Root = new UILinelement()
            {
                From = from.VisualElement,
                To = to.VisualElement,
                FromColor = from.Color,
                ToColor = to.Color,
            };

            from.Link = this;
            to.Link = this;

            StyleSheet customUss = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            Root.styleSheets.Add(customUss);

            Root.AddToClassList("link-container");
            Root.AddToClassList("unity-button");

            NodeEditor.BEventSystem.Listen<OnNodeMoved>(HandleNodeMoved);
            NodeEditor.BEventSystem.Listen<OnNodeResized>(HandleNodeResized);
        }

        private void HandleNodeResized(OnNodeResized evt)
        {
            if (evt.Node != From.ParentNode && evt.Node != To.ParentNode)
                return;

            Refresh();
        }

        private void HandleNodeMoved(OnNodeMoved evt)
        {
            if (evt.Node != From.ParentNode && evt.Node != To.ParentNode)
                return;

            Refresh();
        }

        public void AfterAddToCanvas()
        {
            Refresh();
        }

        public void BeforeRemoveFromCanvas()
        {
            NodeEditor.BEventSystem.Unlisten<OnNodeMoved>(HandleNodeMoved);
            NodeEditor.BEventSystem.Unlisten<OnNodeResized>(HandleNodeResized);

            From = null;
            To = null;
        }

        public void Refresh()
        {
            Root.Refresh();
        }
    }
}
