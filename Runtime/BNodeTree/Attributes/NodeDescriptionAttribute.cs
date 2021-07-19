using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeDescriptionAttribute : Attribute
    {
        public string Description { get; }
        public NodeDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}