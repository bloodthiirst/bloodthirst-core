using Sirenix.OdinInspector;
using UnityEngine;

namespace Bloodthirst.Core.SceneManager
{
    [InlineEditor(InlineEditorModes.FullEditor, DrawHeader = false)]
    public struct ReadOnlyString
    {
        [ShowInInspector]
        [HorizontalGroup]
        [HideLabel]
        [ReadOnly]
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