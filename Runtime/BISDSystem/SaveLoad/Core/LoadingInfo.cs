namespace Bloodthirst.Core.BISDSystem
{
    public class LoadingInfo
    {
        public IGameStateLoader Loader { get; set; }
        public ISavableGameSave GameData { get; set; }
        public ISavableState State { get; set; }
        public ISavable Instance { get; set; }
    }
}
