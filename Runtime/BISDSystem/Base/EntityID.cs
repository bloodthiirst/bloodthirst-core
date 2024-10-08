namespace Bloodthirst.Core.BISDSystem
{
    public class EntityID
    {
        private static int entityCount;

        public static int EntityCount => entityCount;

        public static void Reset(int count)
        {
            entityCount = count;
        }

        public static int GetNextId()
        {
            return entityCount++;
        }
    }
}
