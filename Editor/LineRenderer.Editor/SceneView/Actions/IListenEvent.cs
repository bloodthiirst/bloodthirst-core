namespace Bloodthirst.Editor.UI
{
    internal interface IListenEvent
    {
        void Setup();
        void OnSceneGUI();
        void Destroy();
    }
}
