using System;
using System.Collections;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public interface IUIWindow
    {
        IWindowLayer Manager { get; }
        RectTransform ParentTransform { get; }
        bool IsOpen { get; set; }
        bool RequestOpen { get; set; }
        bool RequestFocus { get; set; }
        bool RequestUnfocus { get; set; }
        bool RequestClose { get; set; }
        bool IsFocused { get; set; }

        event Action<IUIWindow> OnOpen;
        event Action<IUIWindow> OnFocus;
        event Action<IUIWindow> OnUnfocus;
        event Action<IUIWindow> OnClose;
        IEnumerator Open();
        IEnumerator Close();
        IEnumerator Focus();
        IEnumerator Unfocus();
        void TriggerOpen();
        void TriggerClose();
        void TriggerUnfocus();
        void TriggerFocus();
    }
}
