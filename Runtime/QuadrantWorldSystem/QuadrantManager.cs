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
    public class QuadrantManager
    {
        public event Action<IQuadrantEntity> OnEntityAdded;

        public event Action<(int,int,int) , IQuadrantEntity> OnEntityRemoved;

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
                    OnCubeResized();
                }
            }
        }

        /// <summary>
        /// Container for all the entities that need to be grouped by cube
        /// </summary>
        private Dictionary<ValueTuple<int, int, int>, HashSet<IQuadrantEntity>> quadrantCollection;

        public IReadOnlyDictionary<ValueTuple<int, int, int>, HashSet<IQuadrantEntity>> QuadrantColllection => quadrantCollection;

        public QuadrantManager()
        {
            quadrantCollection = new Dictionary<(int, int, int), HashSet<IQuadrantEntity>>();
        }


        public void Clear()
        {
            foreach(KeyValuePair<(int, int, int), HashSet<IQuadrantEntity>> kv in quadrantCollection)
            {
                foreach(IQuadrantEntity e in kv.Value.ToList())
                {
                    Remove(e.QuandrantId, e);
                }
            }

            quadrantCollection.Clear();

        }

        private void OnCubeResized()
        {
            // cache previous entities
            List<IQuadrantEntity> list = new List<IQuadrantEntity>();
            foreach(KeyValuePair<(int, int, int), HashSet<IQuadrantEntity>> kv in quadrantCollection)
            {
                list.AddRange(kv.Value);
            }

            quadrantCollection.Clear();

            foreach(IQuadrantEntity e in list)
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
        public HashSet<IQuadrantEntity> this[int x, int y, int z]
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
                quadrantCollection.Add((x, y, z), new HashSet<IQuadrantEntity>());
            }
        }

        /// <summary>
        /// Add an entity to the system and return the cube id 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (int,int,int) Add(IQuadrantEntity entity)
        {
            Vector3 pos = entity.Postion;

            (int, int, int) id = PositionToId(pos);

            entity.QuandrantId = id;

            this[id.Item1, id.Item2, id.Item3].Add(entity);

            OnEntityAdded?.Invoke(entity);

            entity.OnPositionChanged -= OnPositionChanged;
            entity.OnPositionChanged += OnPositionChanged;

            return id;
        }

        private void OnPositionChanged(IQuadrantEntity entity)
        {
            (int, int, int) newId = Update(entity);
            entity.QuandrantId = newId;
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
        public void Remove((int,int,int)? id , IQuadrantEntity entity)
        {
            if (id.HasValue)
            {
                this[id.Value.Item1, id.Value.Item2, id.Value.Item3].Remove(entity);
            }
            entity.QuandrantId = null;

            entity.OnPositionChanged -= OnPositionChanged;

            OnEntityRemoved?.Invoke(id.Value , entity);
        }

        /// <summary>
        /// Update the entity id in the quadrant world and get the new id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (int,int,int) Update(IQuadrantEntity entity)
        {
            (int, int, int) newId = PositionToId(entity.Postion);

            if(entity.QuandrantId == newId)
            {
                return newId;
            }

            (int, int, int)? oldId = entity.QuandrantId;

            entity.QuandrantId = newId;

            if (oldId.HasValue && oldId.Value != newId)
            {
                Remove(oldId.Value, entity);
            }

            return Add(entity);
        }
    }
}
