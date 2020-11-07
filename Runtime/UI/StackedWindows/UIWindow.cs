using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public abstract class UIWindow : MonoBehaviour , IUIWindow
    {
        public IStackedWindowManager Manager => GetManager();

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
        public void TriggerOpen()
        {
            IsOpen = true;
            ((IUIWindow)this).Manager.Add(this);
            Open();
            OnOpen?.Invoke(this);
        }

        [Button]
        public void TriggerClose()
        {
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
