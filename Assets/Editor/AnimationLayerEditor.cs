using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationLayer))]
public class AnimationLayerEditor : PropertyDrawer
{
    /// <summary>
    /// This is how you draw to the inspector. You aren't really limited to any specific thing 
    /// as images can be drawn. The only limit is how terrible is it to try to place everything
    /// just so. Not to mention you still have to figure out the height of the stuff you just 
    /// drew. Probably the worst API I've seen from Unity.
    /// </summary>
    /// <param name="a_Position"> The position this property should start drawing to </param>
    /// <param name="a_Property"> The property to be drawn </param>
    /// <param name="a_Label"> The name of the property. Seems useless </param>
    public override void OnGUI(Rect a_Position, SerializedProperty a_Property, GUIContent a_Label)
    {
        // Store the value of the original indent to be restored later
        int originalIndent = EditorGUI.indentLevel;

        string originalName = a_Property.displayName;
        int originalDepth = a_Property.depth;

        // Let Unity know you are starting this property
        EditorGUI.BeginProperty(a_Position, a_Label, a_Property);
        {
            if (originalDepth != 0)
                a_Position.y += 3f;

            a_Property.Next(true);

            if (a_Property.stringValue.Length == 0)
            {
                // if this property is part of an array
                if (originalName.Length > 7 && originalName.Substring(0, 7) == "Element")
                {
                    // Parse the array index from the string Unity gives you
                    int parsedIndex = int.Parse(originalName.Substring(8)) + 1;

                    // Show "Animation 'parsedIndex' Delay" since the 'delayTime' variable 
                    // is going to be displayed in the foldout's label
                    a_Property.stringValue = "Layer " + parsedIndex + " Delay";
                }
                else
                    a_Property.stringValue = originalName + " Delay";
            }

            // Setup the Rect 'foldoutRect' with the current value of 'a_Position'
            Rect foldoutRect = a_Position;

            // Set the size of the foldout, create it, and store its return value into this property
            foldoutRect.size = new Vector2(Globals.FOLDOUT_WIDTH, Globals.DEFAULT_HEIGHT);
            a_Property.isExpanded = EditorGUI.Foldout(foldoutRect, a_Property.isExpanded, " ");

            Rect nameRect = a_Position;

            nameRect = EditorGUI.PrefixLabel(nameRect, new GUIContent(" "));

            nameRect.width = nameRect.x - a_Position.x - Globals.SPACING_WIDTH * 2;
            nameRect.height = Globals.DEFAULT_HEIGHT;
            nameRect.position = new Vector2(a_Position.position.x + Globals.SPACING_WIDTH, a_Position.y);

            string previousName = a_Property.stringValue;
            a_Property.stringValue = EditorGUI.TextField(nameRect, a_Property.stringValue);
            if (previousName != a_Property.stringValue && a_Property.stringValue.Length != 0)
                a_Property.stringValue += " Delay";

            // if the foldout is expanded by the user
            if (a_Property.isExpanded)
            {
                // Copy the current property iterator into a new variable to be restored later
                SerializedProperty originalProperty = a_Property.Copy();

                // Move to the 'AnimationData' array: 'animationDataList'
                a_Property.Next(false);
                a_Property.Next(true);
                a_Property.Next(true);

                // Create a new Rect to store the position of the buttons, and set their size
                Rect buttonRect = a_Position;
                buttonRect.size = new Vector2(Globals.BUTTON_WIDTH, Globals.DEFAULT_HEIGHT);

                // Set the position of and create button that will decrease the size of the 'animationDataList' array
                if (originalDepth == 0)
                    buttonRect.x = Screen.width - buttonRect.width - Globals.SPACING_WIDTH;
                else
                    buttonRect.x =
                        Screen.width - buttonRect.width - Globals.SPACING_WIDTH - Globals.NEGATIVE_LIST_SPACE;
                if (GUI.Button(buttonRect, new GUIContent("-", "Remove Animation Layer")))
                    a_Property.arraySize--;

                // Set the position of and create button that will decrease the size of the 'animationDataList' array
                buttonRect.x -= buttonRect.width + Globals.SPACING_WIDTH;
                if (GUI.Button(buttonRect, new GUIContent("+", "Add Animation Layer")))
                    a_Property.arraySize++;

                // Go back to the original property
                a_Property = originalProperty;

                EditorGUI.indentLevel++;

                // Move to the 'delayTime' property
                a_Property.Next(false);

                // Set the size of the 'delayTime' property
                a_Position.width -= Globals.BUTTON_WIDTH * 2 + Globals.SPACING_WIDTH * 3;
                a_Position.height = Globals.DEFAULT_HEIGHT;

                // Draw the 'delayTime' property with no label
                EditorGUI.PropertyField(a_Position, a_Property, new GUIContent(" "));

                // Move to the 'AnimationData' array: 'animationDataList'
                a_Property.Next(true);
                a_Property.Next(true);

                // Move down past the 'delayTime' property
                a_Position.y += Globals.SPACING_HEIGHT;

                for (int i = 0; i < a_Property.arraySize; ++i)
                {
                    // Draw the 'AnimationData' according to my custom drawer
                    EditorGUI.PropertyField(a_Position, a_Property.GetArrayElementAtIndex(i));

                    // Add height depending on if this property is expanded
                    if (a_Property.GetArrayElementAtIndex(i).isExpanded)
                        // Let the child property do all the hard work to calculate the next y value
                        // In this case, it calls 'AnimationDataEditor.GetPropertyHeight' and 
                        // passes this property
                        a_Position.y += EditorGUI.GetPropertyHeight(a_Property.GetArrayElementAtIndex(i));
                    else
                        // Otherwise just add the default
                        a_Position.y += Globals.SPACING_HEIGHT;
                }
            }
            else
            {
                // Move to the 'delayTime' property
                a_Property.Next(false);

                // Set the size of the 'delayTime' property
                a_Position.height = Globals.DEFAULT_HEIGHT;

                // Draw the 'delayTime' property with no label
                EditorGUI.PropertyField(a_Position, a_Property, new GUIContent(" "));
            }
        }
        // Let Unity know you are finished with the property
        EditorGUI.EndProperty();

        // Restore the original indent value
        EditorGUI.indentLevel = originalIndent;
    }
    /// <summary>
    /// This is where you will need to parse the property height. It's not as easy as it sounds
    /// when you have resizing elements based on conditions and other sizes. This is because this 
    /// object cannot store any member variables reliably even though it's not a static object.
    /// </summary>
    /// <param name="a_Property"> The property that needs it's height parsed from </param>
    /// <param name="a_Label"> The name of the property. Seems useless </param>
    /// <returns></returns>
    public override float GetPropertyHeight(SerializedProperty a_Property, GUIContent a_Label)
    {
        // Sets up a variable to add space to our property
        float extraSpace = Globals.SPACING_HEIGHT - EditorGUIUtility.singleLineHeight;

        a_Property.Next(true);

        // if the foldout is expanded by the user
        if (a_Property.isExpanded)
        {
            a_Property.Next(false);
            a_Property.Next(true);
            a_Property.Next(true);
            //a_Property = a_Property.FindPropertyRelative("animationDataList");
            for (int i = 0; i < a_Property.arraySize; ++i)
                // Let the child property do all the hard work to calculate its height
                // In this case, it calls 'AnimationDataEditor.GetPropertyHeight' and gives it this property
                extraSpace += EditorGUI.GetPropertyHeight(a_Property.GetArrayElementAtIndex(i));
        }
        // return the base height plus our 'extraSpace'
        return base.GetPropertyHeight(a_Property, a_Label) + extraSpace;
    }
}