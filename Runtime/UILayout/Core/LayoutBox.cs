using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    [DebuggerDisplay("{Path,nq}")]
    public class LayoutBox : ILayoutBox
    {
        private string name;
        private LayoutStyle layoutStyle;
        private Bounds padding;
        private Bounds margin;
        private Bounds border;
        private Rect rect;
        private ILayoutBox parentLayout;
        private List<ILayoutBox> children = new List<ILayoutBox>();
        private Matrix4x4 worldToLocalMatrix;
        private Matrix4x4 localToWorldMatrix;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Path
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Name);

                ILayoutBox curr = ParentLayout;

                while (curr != null)
                {
                    sb.Insert(0, '\\');
                    sb.Insert(0, curr.Name);
                    curr = curr.ParentLayout;
                }

                return sb.ToString();
            }
        }
        public LayoutStyle LayoutStyle
        {
            get => layoutStyle;
            set => layoutStyle = value;
        }
        public Bounds Padding { get => padding; set => padding = value; }
        public Bounds Margin { get => margin; set => margin = value; }
        public Bounds Border { get => border; set => border = value; }
        public ref Rect Rect => ref rect;
        public IReadOnlyList<ILayoutBox> ChildLayouts => children;
        public Matrix4x4 WorldToLocalMatrix
        {
            get => worldToLocalMatrix;
            private set => worldToLocalMatrix = value;
        }
        public Matrix4x4 LocalToWorldMatrix
        {
            get => localToWorldMatrix;
            private set => localToWorldMatrix = value;
        }
        public ILayoutBox ParentLayout
        {
            get => parentLayout;
            set => parentLayout = value;
        }

        public void AddChild(ILayoutBox layoutBox)
        {
            layoutBox.ParentLayout = this;
            children.Add(layoutBox);
        }

        public void RemoveChild(ILayoutBox layoutBox)
        {
            layoutBox.ParentLayout = null;
            children.Remove(layoutBox);
        }
        public void PostFlow()
        {
            WorldToLocalMatrix = Matrix4x4.Translate(new Vector3(-Rect.x, -Rect.y, 0));
            LocalToWorldMatrix = Matrix4x4.Translate(new Vector3(Rect.x, Rect.y, 0));
        }

        public void Clear()
        {
            children.Clear();
        }
    }
}
