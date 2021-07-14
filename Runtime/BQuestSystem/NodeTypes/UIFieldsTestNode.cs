using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    [NodeMenuPath("UI Fields Test Node")]
    [NodeName("UI Fields Test")]
    public class UIFieldsTestNode : NodeBase
    {
        public string SomeString { get; set; }
        public int SomeInt { get; set; }
        public bool SomeBool { get; set; }
        public ScriptableObject SomeScriptableObject { get; set; }
        public Direction Direction { get; set; }
    }
}
