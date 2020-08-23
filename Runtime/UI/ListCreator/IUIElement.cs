namespace Bloodthirst.Core.UI
{
    public interface IUIElement<INSTANCE>
    {
        void SetupUI(INSTANCE instance);

        void CleanupUI();
    }
}
