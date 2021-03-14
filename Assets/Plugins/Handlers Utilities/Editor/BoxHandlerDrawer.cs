using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace LeoLuz.XVariables
{
    [CustomPropertyDrawer(typeof(HandledBox))]
    public class HandledBoxDrawer : CircleHandlerDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 64 : 16f;
        }
    }


    }