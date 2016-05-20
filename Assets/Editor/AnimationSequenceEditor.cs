using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AnimationData))]
    public class AnimationSequenceEditor : PropertyDrawer
    {
        private static readonly float BUTTON_WIDTH = EditorGUIUtility.singleLineHeight + 5f;

        private const float BUTTON_SPACING = 5f;

        // Draw the property inside the given rect
        public override void OnGUI(Rect a_Position, SerializedProperty a_Property, GUIContent a_Label)
        {
            int originalIndent = EditorGUI.indentLevel;

            EditorGUI.BeginProperty(a_Position, a_Label, a_Property);
            {
                string name = a_Property.displayName;
                if (a_Property.displayName.Substring(0, 7) == "Element")
                    name = "Animation " + (int.Parse(a_Property.displayName.Substring(8)) + 1);

                a_Position.height = EditorGUIUtility.singleLineHeight;
                a_Position.width = EditorGUIUtility.labelWidth;
                a_Property.isExpanded = EditorGUI.Foldout(a_Position, a_Property.isExpanded, GUIContent.none);
                bool isCompact = !a_Property.isExpanded;


                a_Property = a_Property.FindPropertyRelative("animationType");

                a_Position = EditorGUI.PrefixLabel(a_Position, new GUIContent(name));

                EditorGUI.indentLevel = 0;
                a_Position.size = new Vector2((Screen.width - a_Position.x) / 3, EditorGUIUtility.singleLineHeight);
                a_Property.enumValueIndex =
                    (int)Enum.Parse(
                            typeof(AnimationType),
                            EditorGUI.EnumPopup(a_Position, (AnimationType)a_Property.enumValueIndex).ToString());

                a_Property.Next(true);

                a_Position.position = new Vector2(a_Position.x + a_Position.width + BUTTON_SPACING, a_Position.y);
                EditorGUI.indentLevel = 0;
                Rect curvesAnchor = a_Position;

                for (int i = 0; i < a_Property.arraySize; ++i)
                {
                    if (!isCompact)
                        a_Position = GetCurvesRect(curvesAnchor, a_Property.arraySize, i);
                    else
                    {
                        a_Position.position = new Vector2(
                            curvesAnchor.x + EditorGUIUtility.singleLineHeight * i,
                            a_Position.y);
                        a_Position.size = new Vector2(
                            EditorGUIUtility.singleLineHeight,
                            EditorGUIUtility.singleLineHeight);
                    }
                    EditorGUI.PropertyField(
                        a_Position,
                        a_Property.GetArrayElementAtIndex(i),
                        GUIContent.none);
                }
                a_Position.size = new Vector2(BUTTON_WIDTH, EditorGUIUtility.singleLineHeight);

                a_Position.position = new Vector2(Screen.width - a_Position.width - BUTTON_SPACING, a_Position.y);
                if (GUI.Button(a_Position, new GUIContent("-", "Add Animation Curve")))
                    a_Property.arraySize--;

                a_Position.position = new Vector2(a_Position.x - a_Position.width - BUTTON_SPACING, a_Position.y);
                if (GUI.Button(a_Position, new GUIContent("+", "Add Animation Curve")))
                    a_Property.arraySize++;
            }
            EditorGUI.EndProperty();

            EditorGUI.indentLevel = originalIndent;
        }

        public override float GetPropertyHeight(SerializedProperty a_Property, GUIContent a_Label)
        {
            float extraSpace = 0f;

            if (a_Property.isExpanded)
            {
                a_Property = a_Property.FindPropertyRelative("animationCurves");
                if (a_Property.arraySize > 0)
                {
                    Rect position = EditorGUI.PrefixLabel(
                        new Rect(14, 0, 0, 0),
                        new GUIContent(" "));
                    position.size = new Vector2((Screen.width - position.x) / 3, EditorGUIUtility.singleLineHeight);
                    position.position = new Vector2(position.x + position.width + BUTTON_SPACING, position.y);
                    position = GetCurvesRect(position, a_Property.arraySize, 0);
                    extraSpace += position.height - EditorGUIUtility.singleLineHeight;
                }
            }

            return base.GetPropertyHeight(a_Property, a_Label) + extraSpace;
        }

        private static Rect GetCurvesRect(Rect a_Anchor, int a_Size, int a_Index)
        {
            Rect position = new Rect();

            position.width = (Screen.width - a_Anchor.x - (BUTTON_WIDTH + BUTTON_SPACING) * 2) / a_Size - BUTTON_SPACING;
            position.height = position.width;
            position.position = new Vector2(a_Anchor.x + (position.width + BUTTON_SPACING) * a_Index, a_Anchor.y);

            return position;
        }
    }
}
