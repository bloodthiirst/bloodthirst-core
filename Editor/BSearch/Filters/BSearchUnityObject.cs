using Bloodthirst.Core.TreeList;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bloodthirst.Editor.BSearch
{

    [BSearchFilterName("Unity Object")]
    public class BSearchUnityObject : IBSearchFilter
    {
        [BInspectorIgnore]
        public event Action<IBSearchFilter> OnFilterChanged;

        [BInspectorIgnore]
        private UnityEngine.Object _value;

        public bool ClosestUnityObjectParent { get; set; }
        public bool RemoveValueFromRootList { get; set; }

        public UnityEngine.Object Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnFilterChanged?.Invoke(this);
            }
        }

        bool IBSearchFilter.IsValid()
        {
            return Value != null;
        }
        List<List<ResultPath>> IBSearchFilter.GetSearchResults(List<object> rootItems)
        {
            return SearchByValue(IsEqual , rootItems);
        }
        private bool IsEqual(object instance)
        {
            return Value == instance;
        }

        #region field filters
        private IEnumerable<FieldInfo> SingleFields(object instance)
        {
            Type t = instance.GetType();

            foreach (FieldInfo f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // if unity ref
                if (TypeUtils.IsSubTypeOf(f.FieldType, typeof(UnityEngine.Object)))
                {
                    yield return f;
                }

                // if collection (the case of it veing a "Transform" is taken care of by the previous check
                if (TypeUtils.IsSubTypeOf(f.FieldType, typeof(IEnumerable)))
                {
                    continue;
                }

                yield return f;
            }
        }

        private IEnumerable<FieldInfo> CollectionFields(object instance)
        {
            Type t = instance.GetType();

            foreach (FieldInfo f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                // if unity ref
                if (TypeUtils.IsSubTypeOf(f.FieldType, typeof(UnityEngine.Object)))
                {
                    continue;
                }

                // if collection (the case of it veing a "Transform" is taken care of by the previous check
                if (TypeUtils.IsSubTypeOf(f.FieldType, typeof(IEnumerable)))
                {
                    yield return f;
                }
            }
        }
        #endregion

        public List<List<ResultPath>> SearchByValue(Predicate<object> condition , List<object> rootItems)
        {
            HashSet<object> searchedCache = new HashSet<object>();
            List<List<ResultPath>> results = new List<List<ResultPath>>();

            List<object> rootCpy = rootItems.ToList();
            
            if(RemoveValueFromRootList)
            {
                rootCpy.Remove(Value);
            }

            foreach (object root in rootCpy)
            {
                List<ResultPath> curr = new List<ResultPath>();
                curr.Add(new ResultPath() { FieldName = "Root", FieldType = FieldType.FIELD, FieldValue = root });
                RecursiveSearch(condition, root, searchedCache, curr, results);
            }

            foreach(List<ResultPath> l in results)
            {
                l.Reverse();
            }

            if (results.Count == 0 || !ClosestUnityObjectParent)
                return results;

            // organize in tree
            TreeList<object, ResultPath> tree = new TreeList<object, ResultPath>();

            foreach (List<ResultPath> l in results)
            {
                List<object> keys = new List<object>(l.Count);

                // put the items in reverse
                // starting from the thing we're looking for
                // and back to the root
                for (int i = 0; i < l.Count; i++)
                {
                    ResultPath path = l[i];
                    keys.Add(path.FieldValue);
                }

                tree.GetOrCreateLeaf(keys);

                for (int i = 0; i < l.Count; i++)
                {
                    ResultPath path = l[i];
                    tree.LookForKey(path.FieldValue, out TreeLeafInfo<object, ResultPath> info);
                    info.TreeLeaf.Value = path;
                }
            }

            TreeLeaf<object, ResultPath> firstLeaf = tree.SubLeafs[0];

            List<List<ResultPath>> cleanResults = new List<List<ResultPath>>();

            // simplify to only give the best matches
            foreach (TreeLeaf<object, ResultPath> l in firstLeaf.SubLeafs)
            {
                ResultPath path = l.Value;

                List<ResultPath> curr = new List<ResultPath>() { firstLeaf.Value, path };

                RecrusiveClean(l, curr, cleanResults);
            }


            return cleanResults;
        }
        private void RecrusiveClean(TreeLeaf<object, ResultPath> leaf, List<ResultPath> currentPath, List<List<ResultPath>> results)
        {
            if (leaf.Value.FieldValue is UnityEngine.Object)
            {
                List<ResultPath> res = currentPath.ToList();
                results.Add(res);
                return;
            }

            foreach (TreeLeaf<object, ResultPath> l in leaf.SubLeafs)
            {
                List<ResultPath> curr = currentPath.ToList();
                curr.Add(l.Value);
                RecrusiveClean(l, curr, results);
            }
        }
        private void RecursiveSearch(Predicate<object> condition, object searchTarget, HashSet<object> searchedCache, List<ResultPath> currentPath, List<List<ResultPath>> allResults)
        {
            bool isNull = searchTarget == null;
            bool isCached = false;

            if(!isNull)
            {
                isCached = !searchedCache.Add(searchTarget);
            }
            
            if (isNull && isCached)
                return;

            if (condition(searchTarget))
            {
                allResults.Add(currentPath);
            }

            // if null leave
            if (isNull)
            {
                return;
            }

            // if primitive
            if (TypeUtils.PrimitiveTypes.Contains(searchTarget.GetType()))
            {
                return;
            }

            // if already seen it before leave
            if (isCached)
            {
                return;
            }


            // if unity null leave
            if (searchTarget is UnityEngine.Object unityObj)
            {
                if (unityObj == null)
                {
                    return;
                }
            }

            // if collection
            if (searchTarget is IEnumerable collection)
            {
                return;
            }



            // if is gameObject
            // go into each component and child gameObject
            if (searchTarget is GameObject go)
            {
                // components
                foreach (Component c in go.GetComponents<Component>())
                {
                    List<ResultPath> path = currentPath.ToList();
                    ResultPath toAppend = new ResultPath()
                    {
                        FieldName = $"[Compenent] {c.GetType().Name}",
                        FieldType = FieldType.COMPONENT,
                        FieldValue = c
                    };
                    path.Add(toAppend);
                    RecursiveSearch(condition, c, searchedCache, path, allResults);
                }

                // child transforms
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform curr = go.transform.GetChild(i);

                    List<ResultPath> path = currentPath.ToList();

                    ResultPath toAppend = new ResultPath()
                    {
                        FieldName = $"[Child] {curr.gameObject.name}",
                        FieldType = FieldType.CHILD_OBJECT,
                        FieldValue = curr.gameObject,
                        Index = i
                    };

                    path.Add(toAppend);

                    RecursiveSearch(condition, curr.gameObject, searchedCache, path, allResults);
                }

                return;
            }

            List<FieldInfo> singleFields = SingleFields(searchTarget).ToList();
            List<FieldInfo> collectionFields = CollectionFields(searchTarget).ToList();

            // single fields
            foreach (FieldInfo field in singleFields)
            {
                object val = field.GetValue(searchTarget);

                // continue recursing into field
                {
                    List<ResultPath> path = currentPath.ToList();

                    ResultPath toAppend = new ResultPath()
                    {
                        FieldName = $"{field.Name}",
                        FieldType = FieldType.FIELD,
                        FieldValue = val
                    };

                    path.Add(toAppend);

                    RecursiveSearch(condition, val, searchedCache, path, allResults);
                }
            }

            // collection fields
            foreach (FieldInfo field in collectionFields)
            {
                // chekc the collection itself
                object val = field.GetValue(searchTarget);

                // check the collection itself
                {
                    List<ResultPath> path = currentPath.ToList();

                    ResultPath toAppend = new ResultPath()
                    {
                        FieldName = $"{field.Name}",
                        FieldType = FieldType.FIELD,
                        FieldValue = val
                    };

                    path.Add(toAppend);

                    RecursiveSearch(condition, val, searchedCache, path, allResults);
                }

                if (val == null)
                    return;

                // check the elements of the collection
                {
                    int i = 0;
                    IEnumerable asEnumerable = (IEnumerable)val;

                    foreach (object elem in asEnumerable)
                    {
                        List<ResultPath> path = currentPath.ToList();

                        ResultPath toAppend = new ResultPath()
                        {
                            FieldName = $"{field.Name}",
                            FieldType = FieldType.COLLECTION,
                            FieldValue = elem,
                            Index = i
                        };

                        path.Add(toAppend);

                        RecursiveSearch(condition, elem, searchedCache, path, allResults);
                        i++;
                    }
                }
            }




        }

    }
}