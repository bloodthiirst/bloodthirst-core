using Bloodthirst.Scripts.Core.GamePassInitiator;

namespace Bloodthirst.Core.Setup
{
    public interface IPreGameSetup
    {
        int Order { get; }
        void Execute();
    }
}
