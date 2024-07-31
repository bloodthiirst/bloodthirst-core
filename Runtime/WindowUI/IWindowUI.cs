// ======================================================================
// This file contains proprietary technology owned by Virtual Beings SAS.
// Copyright 2011-2024 Virtual Beings SAS.
// ======================================================================

using System;

namespace Bloodthirst.UI
{
    public enum WindowState
    {
        Open,
        Opening,
        Closed,
        Closing
    }

    /// <summary>
    /// Common interface to facilitate communicating with UI windows/dialogs
    /// </summary>
    public interface IWindowUI
    {
        event Action<IWindowUI> BeforeWindowOpened;
        event Action<IWindowUI> OnWindowOpened;
        event Action<IWindowUI> BeforeWindowClosed;
        event Action<IWindowUI> OnWindowClosed;

        WindowState State { get; }
        void Open();
        void Close();
        void CloseImmediate();
        void OpenImmediate();
    }
}
