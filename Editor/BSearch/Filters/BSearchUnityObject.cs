using Bloodthirst.Core.TreeList;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            return SearchByValue(IsEqual, rootItems);
        }
        private bool IsEqual(object instance)
        {
            GameObject instanceGO = null;
            GameObject valueGO = null;

            if (instance is GameObject)
            {
                instanceGO = (GameObject)instance;
            }

            if (Value is GameObject)
            {
                valueGO = (GameObject)Value;
            }
            // if it's a scene or a scriptableObject
            // then we do normal comaprisson
            if (valueGO == null)
            {
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
                return Value == instance;
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
            }

            if (instanceGO == null)
            {
#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
                return Value == instance;
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast
            }

            if(PrefabUtility.IsAnyPrefabInstanceRoot(valueGO))
            {
                valueGO = PrefabUtility.GetCorrespondingObjectFromSource(valueGO);
            }
            if (PrefabUtility.IsAnyPrefabInstanceRoot(instanceGO))
            {
                instanceGO = PrefabUtility.GetCorrespondingObjectFromSource(instanceGO);
            }

            return valueGO == instanceGO;
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
                    continue;
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

        private FieldType RootType(object instance)
        {
            if(instance is ScriptableObject)
            {
                return FieldType.SCRIPTABLEOBJECT;
            }
            if(instance is GameObject)
            {
                return FieldType.GAMEOBJECT;
            }
            if(instance is SceneAsset)
            {
                return FieldType.SCENE;
            }

            return FieldType.OTHER_UNITY_OBJECT;
        }

        private string RootName(object instance)
        {
            if (instance is UnityEngine.Object unityObj)
            {
                return unityObj.name;
            }

            return instance.ToString();
        }

        public List<List<ResultPath>> SearchByValue(Predicate<object> condition, List<object> rootItems)
        {
            HashSet<object> searchedCache = new HashSet<object>();
            List<List<ResultPath>> results = new List<List<ResultPath>>();

            List<object> rootCpy = rootItems.ToList();

            if (RemoveValueFromRootList)
            {
                rootCpy.Remove(Value);
            }

            foreach (object root in rootCpy)
            {
                List<ResultPath> curr = new List<ResultPath>();

                FieldType fieldType = RootType(root);
                string name = RootName(root);

                curr.Add(new ResultPath() { ValueName = name, ValuePath = fieldType, Value = root });
                RecursiveSearch(condition, root, rootCpy, searchedCache, curr, results);
            }

            foreach (List<ResultPath> l in results)
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
                    keys.Add(path.Value);
                }

                tree.GetOrCreateLeaf(keys);

                for (int i = 0; i < l.Count; i++)
                {
                    ResultPath path = l[i];
                    tree.LookForKey(path.Value, out TreeLeafInfo<object, ResultPath> info);
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
            if (leaf.Value.Value is UnityEngine.Object)
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
        private void RecursiveSearch(Predicate<object> condition, object searchTarget, List<object> rootList, HashSet<object> searchedCache, List<ResultPath> currentPath, List<List<ResultPath>> allResults)
        {
            // if the current instance exists as a root
            // and we found it while starting form another object
            // then we leave
            // since it will eventually be picked up by once it starts in the root
            if (rootList.Contains(searchTarget) && currentPath[0].Value != searchTarget)
                return;

            bool isNull = searchTarget == null;
            bool isCached = false;

            if (!isNull)
            {
                isCached = !searchedCache.Add(searchTarget);
            }

            if (condition(searchTarget))
            {
                allResults.Add(currentPath);
            }

            if (isCached)
            {
                return;
            }

            if (isNull)
            {
                return;
            }

            // if primitive
            if (TypeUtils.PrimitiveTypes.Contains(searchTarget.GetType()))
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

            if (searchTarget is SceneAsset sceneAsset)
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

                Scene scene = SceneManager.GetSceneByPath(scenePath);

                // if scene is not already open , we open it
                if (!scene.IsValid())
                {
                    scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                List<GameObject> sceneObjs = scene.GetRootGameObjects().ToList();

                for (int i = 0; i < sceneObjs.Count; i++)
                {
                    GameObject gameObject = sceneObjs[i];
                    List<ResultPath> path = currentPath.ToList();
                    ResultPath toAppend = new ResultPath()
                    {
                        ValueName = gameObject.name,
                        ValuePath = FieldType.GAMEOBJECT,
                        Value = gameObject
                    };
                    path.Add(toAppend);
                    RecursiveSearch(condition, gameObject, rootList, searchedCache, path, allResults);
                }
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
                        ValueName = c.GetType().Name,
                        ValuePath = FieldType.COMPONENT,
                        Value = c
                    };
                    path.Add(toAppend);
                    RecursiveSearch(condition, c, rootList, searchedCache, path, allResults);
                }

                // child transforms
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform curr = go.transform.GetChild(i);

                    List<ResultPath> path = currentPath.ToList();

                    ResultPath toAppend = new ResultPath()
                    {
                        ValueName = curr.gameObject.name,
                        ValuePath = FieldType.GAMEOBJECT,
                        Value = curr.gameObject,
                        Index = i
                    };

                    path.Add(toAppend);

                    RecursiveSearch(condition, curr.gameObject, rootList, searchedCache, path, allResults);
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
                        ValueName = field.Name,
                        ValuePath = FieldType.FIELD,
                        Value = val
                    };

                    path.Add(toAppend);

                    RecursiveSearch(condition, val, rootList, searchedCache, path, allResults);
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
                        ValueName = field.Name,
                        ValuePath = FieldType.FIELD,
                        Value = val
                    };

                    path.Add(toAppend);

                    RecursiveSearch(condition, val, rootList, searchedCache, path, allResults);
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
                            ValueName = field.Name,
                            ValuePath = FieldType.COLLECTION,
                            Value = elem,
                            Index = i
                        };

                        path.Add(toAppend);

                        RecursiveSearch(condition, elem, rootList, searchedCache, path, allResults);
                        i++;
                    }
                }
            }




        }

    }
}