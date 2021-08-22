namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class BISDInfoContainer
    {
        public string ModelName { get; set; }
        public string ModelFolder { get; set; }
        public BISDInfo Behaviour { get; set; }
        public BISDInfo Instance { get; set; }
        public BISDInfo State { get; set; }
        public BISDInfo Data { get; set; }
        public BISDInfo GameData { get; set; }
        public BISDInfo LoadSaveHandler { get; set; }
    }
}
