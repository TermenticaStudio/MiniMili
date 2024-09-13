using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    public static class ObjectsExtension
    {
        private const string DefaultComponentThrowMessage = "Failed to get component of type {0} on object {1}!";
        public static void DestroyAllChildren(this Transform trans)
        {
            if (trans.childCount == 0)
                return;

            for (int i = 0; i < trans.childCount; i++) Object.Destroy(trans.GetChild(i));
        }
        public static T GetComponentOrThrow<T>(this Component obj, string exceptionMessage = null)
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                exceptionMessage ??= String.Format(DefaultComponentThrowMessage, typeof(T).FullName, obj.name);
                throw new NullReferenceException(exceptionMessage);
            }

            return component;
        }


        public static T GetComponentOrThrow<T>(this GameObject obj, string exceptionMessage = null)
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                exceptionMessage ??= String.Format(DefaultComponentThrowMessage, typeof(T).FullName, obj.name);
                throw new NullReferenceException(exceptionMessage);
            }

            return component;
        }
    }
}