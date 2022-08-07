using Bloodthirst.System.CommandSystem;
using System;

namespace Bloodthirst.Core.Setup
{
    public interface IProgressCommand : ICommandBase
    {
        event Action<IProgressCommand, float, float> OnCurrentProgressChanged;
        string TaskName { get; }
        float CurrentProgress { get; }
    }
}
