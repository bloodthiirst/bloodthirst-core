namespace Bloodthirst.Editor.BSearch
{
    public struct ResultPath
    {
        public FieldType ValuePath { get; set; }
        public string ValueName { get; set; }
        public object Value { get; set; }
        public int Index { get; set; }
        public object Key { get; set; }
    }
}