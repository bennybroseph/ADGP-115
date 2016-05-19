using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AnimationData))]
    public class AnimationSequenceEditor : PropertyDrawer
    {
        private static Vector2 s_Spacing = new Vector2(0, 16);

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            property.Next(true);

            Rect enumRect = new Rect(new Vector2(position.x, position.y), new Vector2(150, 16));
            property.enumValueIndex = (int)Enum.Parse(typeof(AnimationType), EditorGUI.EnumPopup(enumRect, (AnimationType)property.enumValueIndex).ToString());
            //position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            //EditorGUI.indentLevel = 0;
            //EditorGUI.EnumMaskPopup(position, GUIContent.none, (AnimationType)property.enumValueIndex);

            property.Next(true);

            property.Next(true);
            int size = property.arraySize;

            EditorGUI.indentLevel = 5;
            position = EditorGUI.PrefixLabel(position, new GUIContent(" "));
            EditorGUI.indentLevel = 0;
            for (int i = 0; i < size; ++i)
            {
                position = new Rect(
                        new Vector2(position.x, position.y),
                        new Vector2(50, 50));
                
                property.GetArrayElementAtIndex(i).animationCurveValue = EditorGUI.CurveField(position, GUIContent.none, property.GetArrayElementAtIndex(i).animationCurveValue);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 50;
        }
    }
}
