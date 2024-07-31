namespace Bloodthirst.Core.Setup
{
    public interface IAsynOperationWrapper
    {
        bool ShouldExecute();
        IProgressCommand CreateOperation();

        bool ShowLoadingScreen { get; }
    }
}
