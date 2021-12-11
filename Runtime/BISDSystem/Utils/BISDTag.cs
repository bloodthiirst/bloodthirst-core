using System;

namespace Bloodthirst.Core.BISD.CodeGeneration
{
    public enum ClassType
    {
        BEHAVIOUR,
        INSTANCE_MAIN,
        INSTANCE_PARTIAL,
        STATE,
        DATA,
        GAME_SAVE,
        GAME_SAVE_HANDLER
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    public class BISDTag : Attribute
    {
        public string ModelName;

        public ClassType ClassType;

        public BISDTag(string modelName, ClassType classType)
        {
            this.ModelName = modelName;
            this.ClassType = classType;
        }
    }
}
