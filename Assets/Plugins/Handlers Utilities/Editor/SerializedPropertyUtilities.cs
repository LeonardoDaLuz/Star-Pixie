using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace LeoLuz.XVariables
{
    public class SerializedPropertyUtilities
    {

        /// <summary>
        /// Convert serialized property to object value
        /// </summary>
        public static object GetObjectValue(SerializedProperty serializedProperty, object parent = null)
        {
            if (parent == null)
                parent = GetParent(serializedProperty);

            if (parent == null)
            {
                Debug.Log("No Parent in serializedProperty: " + serializedProperty.name);
                return null;
            }

            FieldInfo fieldToCheck = parent.GetType().GetField(serializedProperty.name);

            if (fieldToCheck == null)
            {
                Debug.Log("No Field in serializedProperty: " + serializedProperty.name);
                return null;
            }
            object obj = fieldToCheck.GetValue(parent);

            return obj;
        }

        public static FieldInfo GetFieldInfo(SerializedProperty serializedProperty, object parent)
        {
            if (parent == null)
                parent = GetParent(serializedProperty);

            if (parent == null)
            {
                Debug.Log("No Parent in serializedProperty: " + serializedProperty.name);
                return null;
            }

            return  parent.GetType().GetField(serializedProperty.name);            
        }

        public static object GetObjectValue(FieldInfo fieldToCheck, object parent)
        {
            if (fieldToCheck == null)
            {
                Debug.Log("No Field in serializedProperty: " + fieldToCheck);
                return null;
            }
            object obj = fieldToCheck.GetValue(parent);

            return obj;
        }

        /// <summary>
        ///Get parent regardless if is array parent or class parent.
        /// </summary>

        public static object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public static object GetValue(object source, string name, int index)
        {
            //if (source == null)
            //    return null;

            Array values = (Array)GetValue(source, name);
            if (values == null || values.Length == 0)
                return null;
            var enumerable = values as IEnumerable;
            if (enumerable == null)
                return null;
            //
            //       Debug.Log(enumerable.ToString());
            var enm = enumerable.GetEnumerator();
            //     Debug.Log(enm.ToString());
            if (index >= values.Length)
                return null;

            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }


        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

    }
}