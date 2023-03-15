using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dragon.Core
{
    public static class _DCoreTransformExtensions
    {
        /* Finds a child to this transform by name. Searches not only the first level in the 
         * tree hierarchy of child objects, but all the children, grand children, and so on.  
         */
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            var result = parent.Find(name);

            if (result != null)
                return result;

            for (int i = 0; i < parent.childCount; ++i)
            {
                result = parent.GetChild(i).FindDeepChild(name);
                if (result != null)
                    return result;
            }

            return null;
        }

        /* Traverse all the children of the transform and executes the action on this transform,
         * as well as on all the children.
         */
        public static void TraverseAndExecute(this Transform current, Action<Transform> action)
        {
            action(current);

            for (int i = 0; i < current.childCount; ++i)
            {
                current.GetChild(i).TraverseAndExecute(action);
            }
        }

        /* Traverse all the children of the transform and executes the func on this transform,
         * as well as on all the children. Will return true if all of the funcs returns true.
         */
        public static bool TraverseExecuteAndCheck(this Transform current, Func<Transform, bool> func)
        {
            bool ret = func(current);

            for (int i = 0; i < current.childCount; ++i)
            {
                var temp = current.GetChild(i).TraverseExecuteAndCheck(func);
                if (!temp)
                    ret = false;
            }

            return ret;
        }

        public static void ForEachChild(this Transform transform, Action<Transform> action)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i));
            }
        }

        public static void ForEachChild<P1>(this Transform transform, Action<Transform, P1> action, P1 param1)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), param1);
            }
        }

        public static void ForEachChild<P1, P2>(this Transform transform, Action<Transform, P1, P2> action, P1 param1, P2 param2)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), param1, param2);
            }
        }

        public static void ForEachChild<P1, P2, P3>(this Transform transform, Action<Transform, P1, P2, P3> action, P1 param1, P2 param2, P3 param3)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), param1, param2, param3);
            }
        }

        public static void ForEachChild(this Transform transform, Action<Transform, int> action)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), i);
            }
        }

        public static void ForEachChild<P1>(this Transform transform, Action<Transform, int, P1> action, P1 param1)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), i, param1);
            }
        }

        public static void ForEachChild<P1, P2>(this Transform transform, Action<Transform, int, P1, P2> action, P1 param1, P2 param2)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), i, param1, param2);
            }
        }

        public static void ForEachChild<P1, P2, P3>(this Transform transform, Action<Transform, int, P1, P2, P3> action, P1 param1, P2 param2, P3 param3)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                action(transform.GetChild(i), i, param1, param2, param3);
            }
        }

        public static Transform AddParent(this Transform transform, Transform parent)
        {
            transform.parent = parent;
            return transform;
        }

        public static void SetGlobalScale(this Transform transform,Vector3 scale)
        {
            Transform oldParent = transform.parent;
            Vector3 pos = transform.position;
            transform.parent = null;
            transform.localScale = scale;
            transform.parent = oldParent;
            transform.position = pos;
        }
        
        private static Transform _findInChildren(Transform trans, string name)
        {
            if (trans.name == name)
                return trans;
            else
            {
                Transform found;

                for (int i = 0; i < trans.childCount; i++)
                {
                    found = _findInChildren(trans.GetChild(i), name);
                    if (found != null)
                        return found;
                }

                return null;
            }
        }
        
        private static Transform _findInChildrenContains(Transform trans, string stringPart)
        {
            if (trans.name.Contains(stringPart))
                return trans;
            else
            {
                Transform found;

                for (int i = 0; i < trans.childCount; i++)
                {
                    found = _findInChildrenContains(trans.GetChild(i), stringPart);
                    if (found != null)
                        return found;
                }

                return null;
            }
        }

        /// <summary>
        /// Will not have the grandchilds.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static List<Transform> GetFirstChildren(this Transform transform)
        {
            List<Transform> childs = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                childs.Add(transform.GetChild(i));
            }

            return childs;
        }
        
        /// <summary>
        /// Will not have the grandchilds.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static List<Transform> GetAllChildrenWithTag(this Transform transform, string requiredTag = "")
        {
            List<Transform> childrenList = new List<Transform>();
            transform.InsertChildrenRecursive(childrenList,requiredTag);
            return childrenList;
        }

        public static void InsertChildrenRecursive(this Transform transform,List<Transform> childrenList,string requiredTag = "")
        {
            bool requireTag = !string.IsNullOrEmpty(requiredTag);
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.InsertChildrenRecursive(childrenList,requiredTag);
                if (requireTag)
                {
                    if (child.CompareTag(requiredTag))
                    {
                        childrenList.Add(child);
                    }
                }
                else
                {
                    childrenList.Add(child);
                }
            }
        }

        public static void DoForeachChild(this Transform transform, Action<Transform> action)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                action.Invoke(transform.GetChild(i));
            }
        }

        /// <summary>
        /// Generates garbage!
        /// </summary>
        /// <param name="transform"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetComponentsInFirstChildren<T>(this Transform transform)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                list.AddRange(child.GetComponents<T>());
            }

            return list;
        }

        /// <summary>
        /// Generates garbage!
        /// </summary>
        /// <param name="transform"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetComponentsInChildrenIgnoreSelf<T>(this Transform transform)
        {
            List<T> components = new List<T>();
            DoForeachChild
            (
                transform,
                (t)=>components.AddRange( t.GetComponentsInChildren<T>())
            );
            return components;
        }

        /// <summary>
        /// Recursively searches, children of children too.
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindNamedTransformInChildren(this Transform trans, string name)
        {
            return _findInChildren(trans, name);
        }
        
        public static Transform FindTransformContainsStringInChildren(this Transform trans, string containedString)
        {
            return _findInChildrenContains(trans, containedString);
        }
        
        public static void SetAllScale(this Transform transform, float value)
        {
            transform.localScale = new Vector3(value, value, value);
        }

        public static void SetAllPosition(this Transform transform, float value)
        {
            transform.position = new Vector3(value, value, value);
        }

        public static void SetAllLocalPosition(this Transform transform, float value)
        {
            transform.localPosition = new Vector3(value, value, value);
        }

        public static void SetAllRotation(this Transform transform, float value)
        {
            transform.eulerAngles = new Vector3(value, value, value);
        }

        public static void SetAllLocalRotation(this Transform transform, float value)
        {
            transform.localEulerAngles = new Vector3(value, value, value);
        }

        public static void ResetLocalPosRot(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        
        public static void TeleportWorld(this Transform tr, Transform other)
        {
            tr.position = other.position;
            tr.rotation = other.rotation;
        }
        
        public static void TeleportWorld(this Transform tr, Vector3 position, Quaternion rotation)
        {
            tr.position = position;
            tr.rotation = rotation;
        }

        public static void MoveDelta(this Transform transform, Vector3 deltaPosition)
        {
            transform.position = transform.position + deltaPosition;
        }
        
        

        public static void SetPositionWith(this Transform transform, float? x = null, float? y=null, float? z = null)
        {
            transform.position = new Vector3(x ?? transform.position.x, y ?? transform.position.y, z ?? transform.position.z);
        }

        public static void SetPositionWith(this Transform transform, float? x = null, float? y = null)
        {
            transform.position = new Vector2(x ?? transform.position.x, y ?? transform.position.y);
        }

        public static void SetPositionZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetPositionX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void Flatten(this Transform transform)
        {
            transform.SetPositionY(0);
        }
        
    }
}