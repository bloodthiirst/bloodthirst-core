#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
#if ODIN_INSPECTOR
    [InlineEditor(InlineEditorModes.FullEditor, DrawHeader = false)]
#endif
    public struct ReadOnlyString
    {
        
#if ODIN_INSPECTOR
        [ShowInInspector]
        [HorizontalGroup]
        [HideLabel]        
        [ReadOnly]
#endif

        [SerializeField]
        private string value;

        public string Value { get => value; set => this.value = value; }

        public static implicit operator string(ReadOnlyString readOnly)
        {
            return readOnly.value;
        }

        public static implicit operator ReadOnlyString(string val)
        {
            return new ReadOnlyString() { value = val };
        }
    }
}