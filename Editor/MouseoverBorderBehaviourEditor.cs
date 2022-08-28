using UnityEditor;

[CustomEditor(typeof(MouseoverBorderBehaviour))]
[CanEditMultipleObjects]
public class MouseoverBorderBehaviourEditor : Editor
{
    SerializedProperty mouseoverActions;
    SerializedProperty newColor;
    SerializedProperty newWidth;

    private void OnEnable()
    {
        mouseoverActions = serializedObject.FindProperty("mouseoverActions");
        newColor = serializedObject.FindProperty("newColor");
        newWidth = serializedObject.FindProperty("newWidth");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mouseoverActions);

        // Prevent duplication 
        bool newColorDisplayed = false;
        bool newWidthDisplayed = false;

        for (int i = 0; i < mouseoverActions.arraySize; i++)
        {

            switch (mouseoverActions.GetArrayElementAtIndex(i).enumValueIndex)
            {
                case 0: // Color change
                    if (!newColorDisplayed)
                    {
                        EditorGUILayout.PropertyField(newColor);
                        newColorDisplayed = true;
                    }

                    break;

                case 1: // Width change
                    if (!newWidthDisplayed)
                    {
                        EditorGUILayout.PropertyField(newWidth);
                        newWidthDisplayed = true;
                    }
                    break;

            }

        }

        serializedObject.ApplyModifiedProperties();
    }
}