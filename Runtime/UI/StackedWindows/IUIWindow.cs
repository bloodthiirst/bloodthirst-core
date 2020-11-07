using System;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public interface IUIWindow
    {
        IStackedWindowManager Manager { get; }
        RectTransform ParentTransform { get; }
        bool IsOpen { get; set; }
        bool IsHidden { get; set; }
        bool HideInStack { get; }

        event Action<UIWindow> OnOpen;

        event Action<UIWindow> OnClose;
        bool RequestClose();
        void Open();
        void Close();
        void Show();
        void Hide();
        void TriggerOpen();
        void TriggerClose();
        void TriggerHide();
        void TriggerShow();
    }
}
