using UnityEngine;

namespace Bloodthirst.System.Quest.Editor
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    [NodeMenuPath("UI Fields Test Node")]
    public class UIFieldsTestNode : NodeBase
    {
        
        public int SomeInt { get; set; }
        public bool SomeBool { get; set; }
        public ScriptableObject SomeScriptableObject { get; set; }
        public Direction Direction { get; set; }
    }
}
