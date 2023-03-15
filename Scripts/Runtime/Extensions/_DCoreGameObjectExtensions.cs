using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Dragon.Core
{
    public static class _DCoreGameObjectExtensions
    {
        // Tries to get a component on the the GameObject. If the component doesn't exists it adds it and return the newly added component.
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() == null ? go.AddComponent<T>() : go.GetComponent<T>();
        }

        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() != null;
        }

        public static void GetInterfaces<T>(this GameObject objectToSearch, out List<T> resultList) where T : class
        {
            MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
            resultList = new List<T>();
            foreach (MonoBehaviour mb in list)
            {
                if (mb is T)
                {
                    //found one
                    resultList.Add((T)((System.Object)mb));
                }
            }
        }

        public static string GetCategoryTag(this GameObject go,string ctag)
        {
            return TagRegistry.GetCategoryTag(ctag, go);
        }
    
        public static string GetCategoryTag(this Component go,string ctag)
        {
            return TagRegistry.GetCategoryTag(ctag, go.gameObject);
        }

        public static T GetCopyOf<T>(this Component comp, T other) where T : Component
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags);
            foreach (var pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
            return comp as T;
        }

        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
        {
            return go.AddComponent<T>().GetCopyOf(toAdd) as T;
        }
        public static T CastObject<T>(this object input)
        {
            return (T)input;
        }

        public static T ConvertObject<T>(this object input)
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }

        public static T GetComponentInParentExcludeSelf<T>(this GameObject gameObject) where T : class
        {
            Transform parent = gameObject.transform.parent;
            T foundObject = null;
            while (parent != null && foundObject == null)
            {
                foundObject = parent.GetComponent<T>();
                parent = parent.parent;
            }

            return foundObject;
        }
    
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
        {
            T component;
            if (gameObject.TryGetComponent(out component))
            {
                return component;
            }
            else
            {
                component = gameObject.AddComponent<T>();
                return component;
            }
        }
    
        /// <summary>
        /// Child search, SLOW!
        /// </summary>
        /// <param name="go"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool HasChild(this GameObject go, GameObject child)
        {
            foreach (Transform tr in go.transform.GetComponentsInChildrenIgnoreSelf<Transform>())
            {
                if (tr == child.transform) return true;
            }

            return false;
        }
    
        public static T Get<T>(this GameObject go)
        {
            return go.GetComponent<T>();
        }

        public static T[] GetAllAndInvoke<T>(this GameObject go, Action<T> action)
        {
            T[] array = go.GetComponents<T>();
            foreach (T comp in array)
            {
                action?.Invoke(comp);
            }

            return array;
        }

        public static bool IsPrefab(this GameObject go)
        {
            return go.scene.rootCount == 0;
        }

        public static void TryGetAndDo<T>(this GameObject gameObject, Action<T> action)
        {
            if (gameObject.TryGetComponent(out T component))
            {
                action.Invoke(component);
            }
        }
    
        public static void TryGetAndDo<T>(this Component comp, Action<T> action)
        {
            if (comp.TryGetComponent(out T component))
            {
                action.Invoke(component);
            }
        }
    
        public static T InterfaceAs<T>(this ICastable castable) where T : class
        {
            return castable as T;
        }
    
        public static T As<T>(this ICastable castable) where T : class
        {
            return castable as T;
        }

        public static T ComponentInterfaceAs<T>(this IUnityComponent componentInterface) where T : class
        {
            if (componentInterface is T returned)
            {
                return returned;
            }

            return componentInterface.GetComponent<T>();
        }

        public static T As<T>(this UnityEngine.Object unityObject) where T : class
        {
            return unityObject as T;
        }
    
        public static T As<T>(this ScriptableObject so) where T : class
        {
            return so as T;
        }
    
        public static List<GameObject> GetChildGameObjects(this GameObject gameObject, bool dontIncludeParent = false)
        {
            List<Transform> childTransforms = new List<Transform>(gameObject.GetComponentsInChildren<Transform>());

            List<GameObject> childGameObjects = new List<GameObject>();

            foreach(Transform child in childTransforms)
            {
                childGameObjects.Add(child.gameObject);
            }

            if (dontIncludeParent)
            {
                childGameObjects.Remove(gameObject);
            }

            return childGameObjects;
        }

        public static List<GameObject> GetChildGameObjects(this Component component)
        {
            List<Transform> childTransforms = new List<Transform>(component.GetComponentsInChildren<Transform>());

            List<GameObject> childGameObjects = new List<GameObject>();

            foreach (Transform child in childTransforms)
            {
                childGameObjects.Add(child.gameObject);
            }

            return childGameObjects;
        }
    
        public static void Destroy(this UnityEngine.Object unityObject)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(unityObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(unityObject);
            }
        }

        public static void DestroyImmediate(this UnityEngine.Object unityObject)
        {
            UnityEngine.Object.DestroyImmediate(unityObject);
        }


        public static void DestroyAll<T>(this List<T> list) where T: UnityEngine.Object
        {
            if (Application.isPlaying)
            {
                foreach(T item in list)
                {
                    Destroy(item);
                }
                list.Clear();
            }
            else
            {
                foreach(T item in list)
                {
                    DestroyImmediate(item);
                }
                list.Clear();
            }
        }

        public static void DestroyAllGameObjects(this List<GameObject> list)
        {
            if (Application.isPlaying)
            {
                foreach (GameObject item in list)
                {
                    UnityEngine.GameObject.Destroy(item);
                }
                list.Clear();
            }
            else
            {
                foreach (GameObject item in list)
                {
                    UnityEngine.GameObject.DestroyImmediate(item);
                }
                list.Clear();
            }
        }

        public static void SetActiveAll(this List<GameObject> gameObjectList, bool active)
        {
            foreach(GameObject go  in gameObjectList)
            {
                go.SetActive(active);
            }
        }
    
        public static void SetLayer(this List<GameObject> gameObjects, string layer)
        {
            foreach(GameObject go in gameObjects)
            {
                go.layer = LayerMask.NameToLayer(layer);
            }
        }
    }
}