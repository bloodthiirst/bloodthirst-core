using JsonDB;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName ="GameStateDb/Database" , fileName = "Database")]
public class GameStateDb : SerializedScriptableObject
{
    [SerializeField]
    [HideInInspector]
    private List<Type> EntitiesTypes { get; set; } = new List<Type>();

    [SerializeField]
    [HideInInspector]
    private Dictionary<Type, int> IndexTracker { get; set; } = new Dictionary<Type, int>();

    [SerializeField]
    [HideInInspector]
    private Dictionary<Type, EntityInfo> EntityInfos { get; set; } = new Dictionary<Type, EntityInfo>();

    [SerializeField]
    [ShowInInspector]
    private Dictionary<Type, HashSet<IDbEntity>> Database { get; set; } = new Dictionary<Type, HashSet<IDbEntity>>();

    public event Action<GameStateDb> OnDatabaseChanged;

    public event Action<IDbEntity> OnEntityAdded;

    public event Action<IDbEntity> OnEntityRemoved;

    public event Action<IDbEntity> OnEntityChanged;

    /// <summary>
    /// Inject the references for all the entities in the Db
    /// </summary>
    private void ApplyChanges()
    {
        foreach (HashSet<IDbEntity> datas in Database.Values)
        {
            foreach (IDbEntity entity in datas)
            {
                InjectSingleReferences(entity);
            }
        }

        foreach (HashSet<IDbEntity> datas in Database.Values)
        {
            foreach (IDbEntity entity in datas)
            {
                InjectMultiReferences(entity);
            }
        }
    }

    private void AddType(Type t)
    {
        if (EntitiesTypes.Contains(t))
            return;

        EntitiesTypes.Add(t);

        IndexTracker.Add(t, 0);

        HashSet<IDbEntity> hashset = new HashSet<IDbEntity>(new DbEntityComparer<IDbEntity>());

        Database.Add(t, hashset);
    }

    public void AddType<T>() where T : IDbEntity
    {
        Type type = typeof(T);

        AddType(type);
    }

    [Button]
    private void Add(IDbEntity t)
    {
        Type type = t.GetType();
        AddType(type);

        IDbEntity casted = t;
        int id = ++IndexTracker[type];
        casted.EntityId = id;
        Database[type].Add(casted);

        ApplyChanges();

        OnDatabaseChanged?.Invoke(this);

        OnEntityAdded?.Invoke(casted);
    }

    public void Add<T>(T t) where T : IDbEntity
    {
        AddType<T>();

        Type type = typeof(T);

        int id = ++IndexTracker[type];
        t.EntityId = id;
        Database[type].Add(t);

        ApplyChanges();

        OnDatabaseChanged?.Invoke(this);

        OnEntityAdded?.Invoke(t);
    }

    /// <summary>
    /// Get entity with id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public T Get<T>(int id) where T : IDbEntity
    {
        Type type = typeof(T);
        return (T)Database[type].FirstOrDefault(e => e.EntityId == id);
    }

    /// <summary>
    /// Does the entity exist ?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool Exists<T>(T t) where T : IDbEntity
    {
        Type type = typeof(T);

        if (!Database.TryGetValue(type, out HashSet<IDbEntity> vals))
        {
            return vals.Contains(t);
        }

        return false;
    }

    /// <summary>
    /// Wire up the references of an entity
    /// </summary>
    /// <param name="entity"></param>
    private void InjectSingleReferences(IDbEntity entity)
    {
        Type entityType = entity.GetType();

        EntityInfo infos = GetEntityInfo(entityType);

        bool changed = false;

        // single refs
        foreach (SingleReferenceProperty referenceProperty in infos.SingleReferenceInjectors)
        {
            IDbReference dbReference = (IDbReference)referenceProperty.PropertyAccessor.GetValue(entity);

            if (LoadSingleDbReference(entityType, entity, dbReference))
            {
                changed = true;
            }
        }

        // trigger if event is entity is changed
        if (changed)
        {
            OnEntityChanged?.Invoke(entity);
        }
    }

    /// <summary>
    /// Wire up the references of an entity
    /// </summary>
    /// <param name="entity"></param>
    private void InjectMultiReferences(IDbEntity entity)
    {
        Type type = entity.GetType();

        EntityInfo infos = GetEntityInfo(type);


        bool changed = false;

        // single refs
        foreach (MultipleReferenceProperty referenceProperty in infos.MultipleReferenceInjectors)
        {
            IList list = (IList)referenceProperty.PropertyAccessor.GetValue(entity);

            if (LoadDbMultiReference(entity, list))
            {
                changed = true;
            }
        }

        // trigger if event is entity is changed
        if (changed)
        {
            OnEntityChanged?.Invoke(entity);
        }
    }


    private IDbEntity FindEntity(Type entityType, int id)
    {
        return Database[entityType].FirstOrDefault(e => e.EntityId == id);
    }

    /// <summary>
    /// Loads the refrence for the entity
    /// return true if the entity reference changed
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dbReference"></param>
    /// <returns></returns>
    private bool LoadDbMultiReference(IDbEntity entity, IList dbMultiRefs)
    {
        // if null
        // do nothing
        if (dbMultiRefs == null)
            return false;

        bool changed = false;

        for (int i = dbMultiRefs.Count - 1; i >= 0; i--)
        {
            IDbReference reference = (IDbReference)dbMultiRefs[i];

            IDbEntity otherRef = FindEntity(reference.EntityType, reference.ReferenceId.Value);

            // if other ref is null
            // that means that the link between the entities is gone
            // so delete from the list
            if (otherRef == null)
            {
                dbMultiRefs.RemoveAt(i);
                changed = true;
            }
        }

        return changed;
    }


    /// <summary>
    /// Loads the refrence for the entity
    /// return true if the entity reference changed
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="dbReference"></param>
    /// <returns></returns>
    private bool LoadSingleDbReference(Type entityType, IDbEntity entity, IDbReference dbReference)
    {
        // if null
        // do nothing
        if (dbReference == null)
            return false;

        // if refrence id doesn't exist
        // then set to null
        if (!dbReference.ReferenceId.HasValue && dbReference.Entity != null)
        {
            dbReference.Entity = null;
            return true;
        }

        // else if id exists but instance isn't linked
        // then inject the value from the db
        else if (dbReference.ReferenceId.HasValue)
        {
            IDbEntity searchReference = null;

            // if entity is null
            // or if different id
            // then load from db
            if (dbReference.Entity == null || dbReference.Entity.EntityId != dbReference.ReferenceId)
            {
                searchReference = Database[dbReference.EntityType].FirstOrDefault(e => e.EntityId == dbReference.ReferenceId.Value);

                if (searchReference == null)
                {
                    dbReference.ReferenceId = null;
                    throw new Exception($"Entity doesn't exist ( type : { dbReference.EntityType.Name } , id : { dbReference.ReferenceId.Value } )");
                }
            }
            else
            {
                searchReference = dbReference.Entity;
            }

            List<MultipleReferenceProperty> otherMultiRef = EntityInfos[dbReference.EntityType].MultipleReferenceInjectors.Where(i => i.ReferenceType == entityType).ToList();

            if (otherMultiRef != null)
            {
                foreach (MultipleReferenceProperty mutli in otherMultiRef)
                {
                    IList multiRefList = (IList)mutli.PropertyAccessor.GetValue(searchReference);

                    // if the multi list doesn't already contain a reference to the current entity
                    // add a new reference to the list

                    if (multiRefList.Cast<IDbReference>().FirstOrDefault(r => r.ReferenceId == entity.EntityId) == null)
                    {
                        IDbReference reference = mutli.ReferenceCreator();
                        reference.Entity = entity;
                        reference.ReferenceId = entity.EntityId;

                        multiRefList.Add(reference);
                    }
                }
            }

            List<SingleReferenceProperty> otherSingleRef = EntityInfos[dbReference.EntityType].SingleReferenceInjectors.Where(i => i.ReferenceType == entityType).ToList();

            if (otherSingleRef != null)
            {
                foreach (SingleReferenceProperty single in otherSingleRef)
                {
                    IDbReference singleReference = (IDbReference)single.PropertyAccessor.GetValue(searchReference);
                    if (singleReference.ReferenceId != entity.EntityId)
                    {
                        singleReference.Entity = entity;
                        singleReference.ReferenceId = entity.EntityId;
                    }
                }
            }


            dbReference.Entity = searchReference;
            return true;
        }


        return false;
    }

    private EntityInfo GetEntityInfo(Type type)
    {
        EntityInfo infos;

        if (!EntityInfos.TryGetValue(type, out infos))
        {
            List<PropertyInfo> singleReferences = type
                .GetProperties()
                .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(IDbReference)))
                .ToList();

            List<PropertyInfo> multipleReferences = type
                .GetProperties()
                .Where(p => p.PropertyType.GetInterfaces().Contains(typeof(IList)))
                .Where(p => p.PropertyType.GetGenericArguments()[0].GetInterfaces().Contains(typeof(IDbReference)))
                .ToList();

            infos = new EntityInfo();
            infos.EntityType = type;

            List<SingleReferenceProperty> single = new List<SingleReferenceProperty>();
            List<MultipleReferenceProperty> multiple = new List<MultipleReferenceProperty>();

            foreach (PropertyInfo s in singleReferences)
            {
                SingleReferenceProperty reference = new SingleReferenceProperty()
                {
                    PropertyAccessor = s,
                    ReferenceType = s.PropertyType.GetGenericArguments()[0],
                    ReferenceCreator = Utils.CreateDefaultConstructor<IDbReference>(s.PropertyType)
                };

                single.Add(reference);
            }

            foreach (PropertyInfo s in multipleReferences)
            {
                MultipleReferenceProperty reference = new MultipleReferenceProperty()
                {
                    PropertyAccessor = s,
                    ReferenceType = s.PropertyType.GetGenericArguments()[0].GetGenericArguments()[0],
                    ReferenceCreator = Utils.CreateDefaultConstructor<IDbReference>(s.PropertyType.GetGenericArguments()[0])
                };

                multiple.Add(reference);
            }

            infos.SingleReferenceInjectors = single;
            infos.MultipleReferenceInjectors = multiple;

            EntityInfos.Add(type, infos);
        }

        return infos;
    }

    /// <summary>
    /// Remove entity from Db
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public void Remove<T>(T t) where T : IDbEntity
    {
        AddType<T>();
        Type type = typeof(T);
        Database[type].Remove(t);

        // check entity references
        foreach (KeyValuePair<Type, HashSet<IDbEntity>> kv in Database)
        {
            Type entityType = kv.Key;

            EntityInfo infos = GetEntityInfo(entityType);

            foreach (IDbEntity entity in kv.Value)
            {

                bool changed = false;

                // single refs
                foreach (SingleReferenceProperty referenceProperty in infos.SingleReferenceInjectors)
                {
                    IDbReference dbReference = (IDbReference)referenceProperty.PropertyAccessor.GetValue(entity);

                    if (dbReference.ReferenceId == t.EntityId && dbReference.EntityType == type)
                    {
                        dbReference.ReferenceId = null;
                        dbReference.Entity = null;

                        changed = true;
                    }
                }

                // multiple refs
                foreach (MultipleReferenceProperty referenceProperty in infos.MultipleReferenceInjectors)
                {
                    IList dbReference = (IList)referenceProperty.PropertyAccessor.GetValue(entity);

                    if (dbReference == null)
                        continue;


                    for (int i = dbReference.Count - 1; i >= 0; i--)
                    {
                        IDbReference reference = (IDbReference)dbReference[i];

                        if (reference.ReferenceId == t.EntityId && reference.EntityType == type)
                        {
                            dbReference.RemoveAt(i);
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    OnEntityChanged?.Invoke(entity);
                }
            }
        }

        OnDatabaseChanged?.Invoke(this);

        OnEntityRemoved?.Invoke(t);
    }
}
