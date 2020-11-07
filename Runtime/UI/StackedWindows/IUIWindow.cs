using System;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public interface IUIWindow
    {
        IStackedWindowManager Manager { get; }
        RectTransform ParentTransform { get; }
        bool IsOpen { get; set; }

        event Action<UIWindow> OnOpen;

        event Action<UIWindow> OnClose;
        bool RequestClose();
        void Open();
        void Close();

        void TriggerOpen();
        void TriggerClose();
    }
}
