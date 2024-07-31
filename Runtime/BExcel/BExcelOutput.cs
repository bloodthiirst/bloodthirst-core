using System;
using UnityEngine;

namespace Bloodthirst.Editor.BExcelEditor
{
    public class BExcelOutput : ScriptableObject
    {
        public string scriptPath;
        public string scriptNamespace;

        public string[] languages;

        public Tab[] tabs;

        [Serializable]
        public class Row
        {
            public string key;
            public string[] entries;
        }

        [Serializable]
        public class Tab
        {
            public string tabName;
            public Row[] rows;
        }
    }
}
