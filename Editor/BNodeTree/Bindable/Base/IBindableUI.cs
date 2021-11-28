﻿using Bloodthirst.System.Quest.Editor;
using System.Reflection;
using UnityEngine.UIElements;

namespace Bloodthirst.System.Quest.Editor
{
    public interface IBindableUI
    {
        VisualElement VisualElement { get; set; }

        MemberInfo MemberInfo { get; }

        void Setup(object instance, MemberInfo member);
        void CleanUp();
    }
}