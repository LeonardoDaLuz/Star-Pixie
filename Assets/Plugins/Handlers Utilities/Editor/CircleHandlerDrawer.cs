using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;



namespace LeoLuz.XVariables
{
    [CustomPropertyDrawer(typeof(HandledCircle))]
    public class CircleHandlerDrawer : PropertyDrawer
    {

        bool isVisible;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object parent = SerializedPropertyUtilities.GetParent(property);
            FieldInfo fieldInfo = SerializedPropertyUtilities.GetFieldInfo(property, parent);
            HandledArea handledArea = (HandledArea)SerializedPropertyUtilities.GetObjectValue(property);
            MonoBehaviour monoBehaviour = ((MonoBehaviour)property.serializedObject.targetObject);
            handledArea.color = Color.red;

            #region RelativeToTransformCheck
            bool _relativeToTransform = false;
            var Attributes = fieldInfo.GetCustomAttributes(false);

            for (int i = 0; i < Attributes.Length; i++)
            {
                if (Attributes[i].GetType() == typeof(RelativeToTransformAttribute))
                {
                    _relativeToTransform = true;
                }
                else if (Attributes[i].GetType() == typeof(HandlerColorAttribute))
                {
                    var teste = (HandlerColorAttribute)Attributes[i];
    
                    handledArea.color= teste.color;
                }
            }
            #endregion


            if(_relativeToTransform)
                handledArea.transform = monoBehaviour.transform;
            else
                handledArea.transform = null;

            handledArea.serializedObjectMonoBehaviour = (MonoBehaviour)property.serializedObject.targetObject;
            handledArea.label = property.displayName;
            handledArea.gameObject = monoBehaviour.gameObject;




            if (property.isExpanded)
            {
                SceneView.onSceneGUIDelegate -= handledArea.handleOnSceneGUI;
                SceneView.onSceneGUIDelegate -= handledArea.handleOnSceneGUI;

                SceneView.onSceneGUIDelegate += handledArea.handleOnSceneGUI;
            } else
            {
                SceneView.onSceneGUIDelegate -= handledArea.handleOnSceneGUI;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "Handled Area Edited");
            }
            //    EditorUtility.SetDirty(property.serializedObject.targetObject);
            //  property.serializedObject.ApplyModifiedProperties();
            //  EditorUtility.SetDirty(property.serializedObject.targetObject);
            // Undo.RecordObject(property.serializedObject.targetObject, "Area Handled");

            // UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            //property.serializedObject.Update();


        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 48f : 16f;
        }
    }





}