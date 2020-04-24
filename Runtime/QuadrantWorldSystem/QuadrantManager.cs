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
    public class QuadrantManager<TEntity> where TEntity : MonoBehaviour
    {
        /// <summary>
        /// The size of the cube that will be used to divide the world
        /// </summary>
        public readonly float CubeSize;

        /// <summary>
        /// Container for all the entities that need to be grouped by cube
        /// </summary>
        private Dictionary<ValueTuple<int, int, int>, List<TEntity>> quadrantCollection;

        public QuadrantManager(float CubeSize)
        {
            this.CubeSize = CubeSize;

            quadrantCollection = new Dictionary<(int, int, int), List<TEntity>>();
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
                if (!quadrantCollection.ContainsKey((x, y, z)))
                {
                    quadrantCollection.Add((x, y, z), new List<TEntity>());
                }

                return quadrantCollection[(x, y, z)];
            }
        }

        /// <summary>
        /// Add an entity to the system and return the cube id 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (int,int,int) Add(TEntity entity)
        {
            Vector3 pos = entity.transform.position;

            (int, int, int) id = PositionToId(pos);

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
            int x = Mathf.FloorToInt(pos.x / CubeSize);
            int y = Mathf.FloorToInt(pos.y / CubeSize);
            int z = Mathf.FloorToInt(pos.z / CubeSize);

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
            (int, int, int) newId = PositionToId(entity.transform.position);

            if(id == newId)
            {
                return id;
            }

            Remove(id, entity);

            return Add(entity);
        }
    }
}
