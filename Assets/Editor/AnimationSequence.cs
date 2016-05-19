using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AnimationSequence))]
    public class AnimationSequenceEditor : PropertyDrawer
    {
        private static Vector2 s_Spacing = new Vector2(0, 25);

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            property = property.FindPropertyRelative("animationLayers");

            property.Next(true);
            property.Next(true);

            int size = property.intValue;
            EditorGUI.IntField(position, size);

            property.Next(false);

            position = new Rect(
                    new Vector2(position.x, position.y + s_Spacing.y),
                    new Vector2(position.width, position.height));

            for (int i = 0; i < size; ++i, property.Next(false))
            {
                position = new Rect(
                    new Vector2(position.x, position.y + i * 30), 
                    new Vector2(position.width, position.height));

                EditorGUI.LabelField(position, property.displayName);
            }
            position = new Rect(
                    new Vector2(position.x, position.y + s_Spacing.y),
                    new Vector2(200, position.height));

            GUI.Button(position, "Add Animation");

            position = new Rect(
                    new Vector2(position.x + 300, position.y),
                    new Vector2(200, position.height));

            GUI.Button(position, "Remove Animation");

            position = new Rect(
                    new Vector2(position.x, position.y + s_Spacing.y),
                    new Vector2(250, position.height));

            EditorGUI.EndProperty();
        }
    }
}
