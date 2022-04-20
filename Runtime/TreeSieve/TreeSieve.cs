using Bloodthirst.System.Quadrant;  
using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.Collections
{
    public class TreeSieve<TEntity, TEnum> where TEnum : Enum
    {
        internal QuadTree<TEnum, List<TEntity>> quadTree;

        internal Dictionary<TEnum, List<TEntity>> flatDictionary;

        internal Dictionary<TEnum, Predicate<TEntity>> sieveConditions;

        public TreeSieve()
        {
            quadTree = new QuadTree<TEnum, List<TEntity>>();
            flatDictionary = new Dictionary<TEnum, List<TEntity>>();
            sieveConditions = new Dictionary<TEnum, Predicate<TEntity>>();
        }

        public SieveBranchBuilder<TEntity, TEnum> AddSieveBranchBuilder()
        {
            SieveBranchBuilder<TEntity, TEnum> builder = new SieveBranchBuilder<TEntity, TEnum>(this);
            return builder;
        }


    }
}
