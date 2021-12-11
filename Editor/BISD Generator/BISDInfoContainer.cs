namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class BISDInfoContainer
    {
        public string ModelName { get; set; }
        public string ModelFolder { get; set; }
        public BISDInfo Behaviour { get; set; }
        public BISDInfo InstanceMain { get; set; }
        public BISDInfo InstancePartial { get; set; }
        public BISDInfo State { get; set; }
        public BISDInfo Data { get; set; }
        public BISDInfo GameSave { get; set; }
        public BISDInfo GameSaveHandler { get; set; }
    }
}
