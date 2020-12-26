using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    /// <summary>
    /// A quadrant system manager that hepls group the game entities by grouping them into cubes in world space
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class QuadrantManager<TEntity> where TEntity : IQuadrantEntity
    {
        private Vector3 cubeSize;
        /// <summary>
        /// The size of the cube that will be used to divide the world
        /// </summary>
        public Vector3 CubeSize
        {
            get => cubeSize;
            set
            {
                if (cubeSize != value)
                {
                    cubeSize = value;
                    Reset();
                }
            }
        }

        /// <summary>
        /// Container for all the entities that need to be grouped by cube
        /// </summary>
        private Dictionary<ValueTuple<int, int, int>, List<TEntity>> quadrantCollection;

        public IReadOnlyDictionary<ValueTuple<int, int, int>, List<TEntity>> QuadrantColllection => quadrantCollection;

        public QuadrantManager()
        {
            quadrantCollection = new Dictionary<(int, int, int), List<TEntity>>();
        }


        public void Clear()
        {
            quadrantCollection.Clear();
        }

        private void Reset()
        {
            // cache previous entities
            List<TEntity> list = new List<TEntity>();
            foreach(KeyValuePair<(int, int, int), List<TEntity>> kv in quadrantCollection)
            {
                list.AddRange(kv.Value);
            }

            quadrantCollection.Clear();

            foreach(TEntity e in list)
            {
                Add(e);
            }
        }

        /// <summary>
        /// Get the list of entities at a specific cube in the world
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public List<TEntity> this[int x, int y, int z]
        {
            get
            {
                TryAddQuadrantCube(x, y, z);

                return quadrantCollection[(x, y, z)];
            }
        }

        public void TryAddQuadrantCube(int x, int y, int z)
        {
            if (!quadrantCollection.ContainsKey((x, y, z)))
            {
                quadrantCollection.Add((x, y, z), new List<TEntity>());
            }
        }

        /// <summary>
        /// Add an entity to the system and return the cube id 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (int,int,int) Add(TEntity entity)
        {
            Vector3 pos = entity.Postion;

            (int, int, int) id = PositionToId(pos);

            entity.QuandrantId = id;

            this[id.Item1, id.Item2, id.Item3].Add(entity);

            return id;
        }

        /// <summary>
        /// Return the id from the world space position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private (int,int,int) PositionToId(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x / CubeSize.x);
            int y = Mathf.FloorToInt(pos.y / CubeSize.y);
            int z = Mathf.FloorToInt(pos.z / CubeSize.y);

            return (x, y, z);
        }

        /// <summary>
        /// remove an entity from the quandrant system
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        public void Remove((int,int,int) id , TEntity entity)
        {
            this[id.Item1, id.Item2, id.Item3].Remove(entity);
        }

        /// <summary>
        /// Update the entity id in the quadrant world and get the new id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (int,int,int) Update((int, int, int) id, TEntity entity)
        {
            (int, int, int) newId = PositionToId(entity.Postion);

            if(id == newId)
            {
                return id;
            }

            entity.QuandrantId = newId;

            Remove(id, entity);

            return Add(entity);
        }
    }
}
