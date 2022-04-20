using System.Collections.Generic;
using UnityEditor;

namespace Bloodthirst.Core.BISDSystem
{
    [InitializeOnLoad]
    public class EntityManager
    {
        private static int entityCount;

        private static int EntityCount => entityCount;

        public static int GetNextId()
        {
            return entityCount++;
        }

        private static List<IEnumerable<IEntityInstance>> _allInstanceSets;

        internal static List<IEnumerable<IEntityInstance>> _AllInstanceSets
        {
            get
            {
                if (_allInstanceSets == null)
                {
                    _allInstanceSets = new List<IEnumerable<IEntityInstance>>();

                }

                return _allInstanceSets;
            }
        }
    }
}
