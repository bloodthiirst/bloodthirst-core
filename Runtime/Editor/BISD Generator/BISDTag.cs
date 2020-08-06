using System;

namespace Assets.Scripts.Game
{

    public enum ClassType
    {
        BEHAVIOUR,
        INSTANCE,
        STATE,
        DATA
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public class BISDTag : Attribute
    {
        public string ModelName;

        public ClassType ClassType;

        public BISDTag(string modelName , ClassType classType)
        {
            this.ModelName = modelName;
            this.ClassType = classType;
        }
    }
}
