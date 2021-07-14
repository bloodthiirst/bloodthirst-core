using Newtonsoft.Json;
using System;

namespace Bloodthirst.System.Quest.Editor
{
    [global::System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeNameAttribute : Attribute
    {
        public string Name { get; }  
        public NodeNameAttribute(string name)
        {
            Name = name;
        }
    }
}