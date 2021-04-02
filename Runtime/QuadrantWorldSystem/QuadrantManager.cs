using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    /// <summary>
    /// A quadrant system manager that hepls group the game entities by grouping them into cubes in world space
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class QuadrantManager<T> where T : IQuadrantEntity<T>
    {
        public event Action<T> OnEntityAdded;

        public event Action<List<int>, T> OnEntityRemoved;

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
        private QuadTreeEquatable<int, T> quadTree;

        public QuadTreeEquatable<int, T> QuadTree => quadTree;

        public QuadrantManager()
        {
            quadTree = new QuadTreeEquatable<int, T>();
        }

        public void Clear()
        {
            quadTree.Clear();

        }

        private void OnCubeResized()
        {
            // cache previous entities
            List<QuadLeafEquatable<int, T>> list = new List<QuadLeafEquatable<int, T>>();

            foreach (QuadLeafEquatable<int, T> l in quadTree.RootLeafs)
            {
                list.AddRange(l.GetAllRecursively());
            }

            quadTree.Clear();

            List<T> elements = list.SelectMany(l => l.Elements).ToList();

            foreach (QuadLeafEquatable<int, T> l in list)
            {
                foreach (T e in l.Elements)
                {
                    e.QuandrantId = null;
                    Add(e);
                }
            }
        }

        /// <summary>
        /// Get the list of entities at a specific cube in the world
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private QuadLeafEquatable<int, T> this[List<int> id]
        {
            get
            {
                return quadTree.Traverse(id);
            }
        }

        /// <summary>
        /// Add an entity to the system and return the cube id 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<int> Add(T entity)
        {
            Vector3 pos = entity.Postion;

            List<int> id = PositionToId(pos);

            entity.QuandrantId = id;

            QuadLeafEquatable<int, T> leaf = this[id];

            leaf.Elements.Add(entity);

            OnEntityAdded?.Invoke(entity);

            entity.OnPositionChanged -= OnPositionChanged;
            entity.OnPositionChanged += OnPositionChanged;

            return id;
        }

        private void OnPositionChanged(T entity)
        {
            List<int> newId = Update(entity);
            entity.QuandrantId = newId;
        }

        /// <summary>
        /// Return the id from the world space position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private List<int> PositionToId(Vector3 pos)
        {
            List<int> lst = new List<int>();

            int x = Mathf.FloorToInt(pos.x / CubeSize.x);
            int y = Mathf.FloorToInt(pos.y / CubeSize.y);
            int z = Mathf.FloorToInt(pos.z / CubeSize.z);

            lst.Add(x);
            lst.Add(y);
            lst.Add(z);


            return lst;
        }

        /// <summary>
        /// remove an entity from the quandrant system
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        public void Remove(List<int> id, T entity)
        {
            if (id != null)
            {
                QuadLeafEquatable<int, T> leaf = this[id];
                leaf.Elements.Remove(entity);
            }
            entity.QuandrantId = null;

            entity.OnPositionChanged -= OnPositionChanged;

            OnEntityRemoved?.Invoke(id, entity);
        }

        /// <summary>
        /// Update the entity id in the quadrant world and get the new id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<int> Update(T entity)
        {
            List<int> newId = PositionToId(entity.Postion);

            if (entity.QuandrantId.IsSame(newId))
            {
                return newId;
            }

            List<int> oldId = entity.QuandrantId;

            entity.QuandrantId = newId;

            if (oldId != null && !oldId.IsSame(newId))
            {
                Remove(oldId, entity);
            }

            return Add(entity);
        }
    }
}
