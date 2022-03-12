namespace Bloodthirst.Runtime.BNodeTree
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NodeMenuPathAttribute : global::System.Attribute
    {
        public string NodePath { get; }

        public NodeMenuPathAttribute(string nodePath)
        {
            NodePath = nodePath;
        }
    }
}
