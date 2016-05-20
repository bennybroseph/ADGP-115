using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Editor
{
    public static class Globals
    {

        public const float ANIMATION_CURVE_HEIGHT = 50;
        public const float ANIMATION_CURVE_WIDTH = ANIMATION_CURVE_HEIGHT;
    }

    [CustomPropertyDrawer(typeof(AnimationLayer))]
    public class AnimationLayerEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int originalIndent = EditorGUI.indentLevel;

            EditorGUI.BeginProperty(position, label, property);
            {
                string name = property.displayName;
                if (property.displayName.Substring(0, 7) == "Element")
                    name = "Animation Layer " + (int.Parse(property.displayName.Substring(8)) + 1);

                position.height = EditorGUIUtility.singleLineHeight;
                property.isExpanded =
                    EditorGUI.Foldout(
                        position,
                        property.isExpanded,
                        new GUIContent(name));

                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    property.Next(true);

                    position.y += EditorGUIUtility.singleLineHeight;
                    position.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(position, property);

                    property.Next(true);
                    property.Next(true);
                    int size = property.arraySize;

                    position.y += 16;
                    position.height = 16;
                    property.isExpanded =
                        EditorGUI.Foldout(
                            position,
                            property.isExpanded,
                            new GUIContent("Animations"));

                    if (property.isExpanded)
                    {
                        position.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < size; ++i)
                        {
                            EditorGUI.PropertyField(position, property.GetArrayElementAtIndex(i));

                            if (property.GetArrayElementAtIndex(i).isExpanded)
                                position.y += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                            else
                                position.y += EditorGUIUtility.singleLineHeight;
                        }
                    }
                }
            }
            EditorGUI.EndProperty();

            EditorGUI.indentLevel = originalIndent;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float extraSpace = 0f;

            if (property.isExpanded)
            {
                extraSpace += EditorGUIUtility.singleLineHeight * 2;

                property.Next(true);
                property.Next(true);

                int size = property.arraySize;

                property.Next(true);
                if (property.isExpanded)
                {
                    for (int i = 0; i < size; ++i)
                        if (property.GetArrayElementAtIndex(i).isExpanded)
                            extraSpace += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                        else
                            extraSpace += EditorGUIUtility.singleLineHeight;
                }
            }

            return base.GetPropertyHeight(property, label) + extraSpace;
        }
    }
}