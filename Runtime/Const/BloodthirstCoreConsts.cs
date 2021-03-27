namespace Bloodthirst.Core.Consts
{
    public static class BloodthirstCoreConsts
    {
        private static string[] order = new string[]
        {
            "TRACK_ASSEMBLY_RELOAD",
            "SINGLETONS_CREATION_CHECK",
            "SINGLETONS_RELOAD",
            "POOL_GENERATOR",
            "SCENE_CREATOR",
            "BISD_OBSERVABLE_GENERATOR"

        };

        #region DidReloadScripts conts

        public const int TRACK_ASSEMBLY_RELOAD = 0;

        public const int SINGLETONS_CREATION_CHECK = 1;

        public const int SINGLETONS_RELOAD = 2;

        public const int POOL_GENERATOR = 3;

        public const int SCENE_CREATOR = 4;

        public const int BISD_OBSERVABLE_GENERATOR = 5;

        public const int EDITOR_OPEN_TRACKER = 6;


        #endregion
    }
}
