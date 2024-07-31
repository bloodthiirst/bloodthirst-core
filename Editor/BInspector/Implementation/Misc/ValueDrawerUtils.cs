using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public static class ValueDrawerUtils
    {
        internal static LabelSpacing spacingStyle = new LabelSpacing();
        internal static LabelDrawer labelDrawer = new LabelDrawer();


        public static Label GetLabel(IValueDrawer valueDrawer)
        {
            VisualElement labelContainer = valueDrawer.DrawerContainer.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS);

            if (labelContainer == null)
                return null;

            return labelContainer.Q<Label>();
        }

        public static string ValuePathAsString(this ValuePath valuePath)
        {
            switch (valuePath.PathType)
            {
                case PathType.ROOT:
                    {
                        return "Root";
                    }
                case PathType.STATIC:
                    {
                        return "Static";
                    }
                case PathType.FIELD:
                    {
                        return valuePath.FieldName;
                    }
                case PathType.LIST_ENTRY:
                    {
                        return $"Element[{valuePath.ListIndex}]";
                    }
                case PathType.DICTIONARY_ENTRY:
                    {
                        return $"Key[{valuePath.DictionaryKey}]";
                    }
                default:
                    {
                        return valuePath.Custom;
                    }
            }
        }


        public static void DoLayoutRoot(IValueDrawer drawer, IValueProvider info)
        {
            Assert.IsTrue(info.ValuePath.PathType == PathType.ROOT);

            LayoutContext ctx = new LayoutContext()
            {
                AllDrawers = new List<IValueDrawer>(),
                IndentationLevel = 0
            };

            drawer.PrepareData(info, null);
            drawer.GenerateContainer();
            drawer.GenerateDrawer(ctx);
            drawer.PostLayout();

            drawer.DrawerContainer.userData = drawer;
            drawer.DrawerContainer.UnregisterCallback<GeometryChangedEvent>(HandleRootDrawerChanged);
            drawer.DrawerContainer.RegisterCallback<GeometryChangedEvent>(HandleRootDrawerChanged);
        }

        public static void DoDestroyRoot(IValueDrawer drawer)
        {
            Assert.IsTrue(drawer.ValueProvider.ValuePath.PathType == PathType.ROOT);

            drawer.DrawerContainer.UnregisterCallback<GeometryChangedEvent>(HandleRootDrawerChanged);

            drawer.Destroy();
        }

        public static void DoLayout(IValueDrawer drawer, IValueDrawer parent, IValueProvider info)
        {
            LayoutContext ctx = new LayoutContext()
            {
                AllDrawers = new List<IValueDrawer>(),
                IndentationLevel = 0
            };

            drawer.PrepareData(info, parent);
            drawer.GenerateContainer();
            drawer.GenerateDrawer(ctx);
            drawer.PostLayout();

            if (parent != null)
            {
                parent.AddChild(drawer);
            }
        }

        private static void HandleRootDrawerChanged(GeometryChangedEvent evt)
        {
            IValueDrawer drawer = (evt.target as VisualElement).userData as IValueDrawer;

            if (drawer == null)
                return;

            ValueDrawerUtils.spacingStyle.Setup(drawer);
        }
    }



}