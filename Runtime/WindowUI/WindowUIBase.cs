// ======================================================================
// This file contains proprietary technology owned by Virtual Beings SAS.
// Copyright 2011-2024 Virtual Beings SAS.
// ======================================================================

using System;
using System.Collections;
using UnityEngine;

namespace Bloodthirst.UI
{
    /// <summary>
    /// <para>This is the main base class for all "window-like" UIs , it offers events , methods that make hooking up UI easier </para>
    /// <para>It also supports overriding animations for opening/closing</para>
    /// </summary>
    public abstract class WindowUIBase : MonoBehaviour, IWindowUI
    {
        [SerializeField]
        private WindowState _state;
        public WindowState State => _state;

        public event Action<IWindowUI> BeforeWindowOpened;

        public event Action<IWindowUI> OnWindowOpened;

        public event Action<IWindowUI> BeforeWindowClosed;

        public event Action<IWindowUI> OnWindowClosed;

        private IWindowUIEffect _windowEffect;

        private Coroutine _handle;

        protected virtual void Awake()
        {
            CloseImmediate();
        }

        protected virtual void BeforeOpen() { }
        protected virtual void AfterOpen() { }
        protected virtual void BeforeClose() { }
        protected virtual void AfterClose() { }


        [ContextMenu(nameof(Close))]
        /// <summary>
        /// <para>Use this method to close the UI using the animations attached to it</para>
        /// </summary>
        public void Close()
        {
            if (State == WindowState.Closing || State == WindowState.Closed)
                return;

            if (_handle != null)
            {
                StopCoroutine(_handle);
            }

            _handle = StartCoroutine(CrtClose());
        }

        [ContextMenu(nameof(Open))]
        /// <summary>
        /// <para>Use this method to open the UI using the animations attached to it</para>
        /// </summary>
        public void Open()
        {
            if (State == WindowState.Opening || State == WindowState.Open)
                return;

            if (_handle != null)
            {
                StopCoroutine(_handle);
            }

            _handle = StartCoroutine(CrtOpen());
        }

        private IEnumerator CrtOpen()
        {
            _state = WindowState.Opening;
            BeforeWindowOpened?.Invoke(this);
            BeforeOpen();

            if (TryGetComponent(out _windowEffect))
            {
                yield return _windowEffect.OpenCrt(this);
            }

            _state = WindowState.Open;
            OnWindowOpened?.Invoke(this);
            AfterOpen();
        }

        private IEnumerator CrtClose()
        {
            _state = WindowState.Closing;
            BeforeWindowClosed?.Invoke(this);
            BeforeClose();

            if (TryGetComponent(out _windowEffect))
            {
                yield return _windowEffect.CloseCrt(this);
            }
            _state = WindowState.Closed;
            OnWindowClosed?.Invoke(this);
            AfterClose();
        }

        /// <summary>
        /// <para>Use this method to close the UI immediately , useful for initialization</para>
        /// </summary>
        public void OpenImmediate()
        {
            _state = WindowState.Opening;
            BeforeWindowOpened?.Invoke(this);
            BeforeOpen();

            if (TryGetComponent(out _windowEffect))
            {
                _windowEffect.OpenImmidiate(this);
            }

            _state = WindowState.Open;
            OnWindowOpened?.Invoke(this);
            AfterOpen();
        }


        /// <summary>
        /// <para>Use this method to open the UI immediately , useful for initialization</para>
        /// </summary>
        public void CloseImmediate()
        {
            _state = WindowState.Closing;
            BeforeWindowClosed?.Invoke(this);
            BeforeClose();

            if (TryGetComponent(out _windowEffect))
            {
                _windowEffect.CloseImmidiate(this);
            }
            _state = WindowState.Closed;
            OnWindowClosed?.Invoke(this);
            AfterClose();
        }
    }
}
