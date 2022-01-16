using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public interface ILayoutBox
    {
        string Name { get; set; }
        string Path { get; }
        ref Rect Rect { get; }
        LayoutStyle LayoutStyle { get; set; }
        Bounds Margin { get; set; }
        Bounds Border { get; set; }
        Bounds Padding { get; set; }

        Matrix4x4 LocalToWorldMatrix { get; }
        Matrix4x4 WorldToLocalMatrix { get; }
        ILayoutBox ParentLayout { get; set; }
        IReadOnlyList<ILayoutBox> ChildLayouts { get; }
        void AddChild(ILayoutBox layoutBox);
        void RemoveChild(ILayoutBox layoutBox);
        void PostFlow();
        void Clear();

    }
}