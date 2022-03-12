using Bloodthirst.Runtime.BNodeTree;
using System.Reflection;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BNodeTree
{
    public interface IBindableUI
    {
        VisualElement VisualElement { get; set; }

        MemberInfo MemberInfo { get; }

        void Setup(object instance, MemberInfo member);
        void CleanUp();
    }
}