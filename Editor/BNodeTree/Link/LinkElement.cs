﻿using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public class LinkElement
    {
        private const string USS_PATH = BNodeTreeEditorUtils.EDITOR_BASE_PATH + "/Link/LinkElement.uss";
        
        public ILinkType LinkType { get; set; }    
        public PortBaseElement From { get; private set; }
        public PortBaseElement To { get; private set; }
        private UILinelement Root { get; set; }
        public VisualElement VisualElement => Root;

        public event Action<LinkElement, ClickEvent> OnLinkClicked;

        public event Action<LinkElement, ClickEvent> OnLinkRequestRemove;

        public LinkElement( ILinkType link, PortBaseElement from, PortBaseElement to)
        {
            From = from;
            To = to;
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

            From.ParentNode.OnNodeMoved -= HandleNodeMoved;
            From.ParentNode.OnNodeMoved += HandleNodeMoved;

            From.ParentNode.OnNodeResized -= HandleNodeResized;
            From.ParentNode.OnNodeResized += HandleNodeResized;

            To.ParentNode.OnNodeMoved -= HandleNodeMoved;
            To.ParentNode.OnNodeMoved += HandleNodeMoved;

            To.ParentNode.OnNodeResized -= HandleNodeResized;
            To.ParentNode.OnNodeResized += HandleNodeResized;
        }

        private void HandleNodeResized(NodeBaseElement node)
        {
            Refresh();
        }

        private void HandleNodeMoved(NodeBaseElement node)
        {
            Refresh();
        }

        public void AfterAddToCanvas()
        {
            Refresh();
        }

        public void BeforeRemoveFromCanvas()
        {
            From.ParentNode.OnNodeMoved -= HandleNodeMoved;
            To.ParentNode.OnNodeMoved -= HandleNodeMoved;

            From.ParentNode.OnNodeResized -= HandleNodeResized;
            To.ParentNode.OnNodeResized -= HandleNodeResized;

            From = null;
            To = null;
        }

        public void Refresh()
        {
            Root.Refresh();
        }
    }
}