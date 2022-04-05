using System.Collections.Generic;

namespace Bloodthirst.Editor.BHotReload
{
    public struct GameObjectSceneData
    {
        public string ScenePath { get; set; }
        public string GameObjectName { get; set; }
        public List<int> SceneGameObjectIndex { get; set; }
        public int ComponentIndex { get; set; }
        public object ComponentValue { get; set; }
    }
}
