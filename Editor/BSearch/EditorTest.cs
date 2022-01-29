using UnityEngine;

namespace Bloodthirst.Editor.BSearch
{
    public class EditorTest
    {
        public GameObject SomeGO { get; set; }
        public int SomeInt { get; set; }

        private EditorTest child;
    }

    public struct StructTest
    {
        public enum EnumEx
        {
            One,Two,Three
        }

        private EnumEx En { get; set; }

        private int id;

    }
}