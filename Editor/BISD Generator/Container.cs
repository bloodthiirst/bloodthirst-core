namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public class Container
    {
        public string ModelName { get; set; }
        public BISDInfo Behaviour { get; set; } = new BISDInfo();
        public BISDInfo Instance { get; set; } = new BISDInfo();
        public BISDInfo State { get; set; } = new BISDInfo();
        public BISDInfo Data { get; set; } = new BISDInfo();
        public BISDInfo GameData { get; set; } = new BISDInfo();
    }
}
