namespace Bloodthirst.Editor.BSearch
{
    public struct ResultPath
    {
        public FieldType FieldType { get; set; }
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
        public int Index { get; set; }
        public object Key { get; set; }
    }
}