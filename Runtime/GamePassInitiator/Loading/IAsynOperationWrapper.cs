using Bloodthirst.System.CommandSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.Core.Setup
{
    public enum AsyncOpState
    {
        WAITING,
        EXECUTING,
        DONE
    };

    public interface IAsynOperationWrapper
    {
        IProgressCommand CreateOperation();
    }

    public interface IProgressCommand : ICommandBase
    {
        event Action<IProgressCommand, float, float> OnCurrentProgressChanged;

        float CurrentProgress { get; }
    }
}
