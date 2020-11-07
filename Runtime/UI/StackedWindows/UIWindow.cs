using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public abstract class UIWindow : MonoBehaviour , IUIWindow
    {
        public IStackedWindowManager Manager => GetManager();
        public abstract bool HideInStack { get; }
        public abstract bool IsHidden { get; set; }
        public bool IsOpen { get; set; }

        [SerializeField]
        protected RectTransform parentTransform;

        public RectTransform ParentTransform => parentTransform;

        public event Action<UIWindow> OnOpen;

        public event Action<UIWindow> OnClose;

        public abstract IStackedWindowManager GetManager();
        public abstract bool RequestClose();
        protected abstract void Open();
        protected abstract void Close();

        [Button]
        public void TriggerHide()
        {
            if (HideInStack)
            {
                IsHidden = true;
                ((IUIWindow)this).Manager.Add(this);
                Hide();
            }
        }

        [Button]
        public void TriggerShow()
        {
            if (HideInStack)
            {
                IsHidden = false;
                ((IUIWindow)this).Manager.Add(this);
                Show();
            }
        }

        public abstract void Show();
        public abstract void Hide();

        [Button]
        public void TriggerOpen()
        {
            if (IsOpen)
            {
                TriggerShow();
                return;
            }
            IsOpen = true;
            ((IUIWindow)this).Manager.Add(this);
            Open();
            OnOpen?.Invoke(this);
        }

        [Button]
        public void TriggerClose()
        {
            if (!IsOpen)
                return;
            IsOpen = false;
            Close();
            ((IUIWindow)this).Manager.Remove(this);
            OnClose?.Invoke(this);
        }

        void IUIWindow.Open()
        {
            Open();
        }

        void IUIWindow.Close()
        {
            Close();
        }
    }
}
