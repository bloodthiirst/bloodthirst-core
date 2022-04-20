using System;
using System.Collections.Generic;

namespace Bloodthirst.Editor.BHotReload
{
    public class GameStateSnapshot
    {
        public List<GameObjectSceneData> SceneObjects { get; set; }

        public List<TypeStaticData> StaticDatas { get; set; }
    }
}
