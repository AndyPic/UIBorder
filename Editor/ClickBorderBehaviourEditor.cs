using UnityEditor;

[CustomEditor(typeof(ClickBorderBehaviour))]
[CanEditMultipleObjects]
public class ClickBorderBehaviourEditor : Editor
{
    SerializedProperty clickActions;
    SerializedProperty newColor;
    SerializedProperty newWidth;
    SerializedProperty actuationType;

    private void OnEnable()
    {
        clickActions = serializedObject.FindProperty("clickActions");
        newColor = serializedObject.FindProperty("newColor");
        newWidth = serializedObject.FindProperty("newWidth");
        actuationType = serializedObject.FindProperty("actuationType");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(clickActions);

        // Prevent duplication 
        bool newColorDisplayed = false;
        bool newWidthDisplayed = false;

        for (int i = 0; i < clickActions.arraySize; i++)
        {

            switch (clickActions.GetArrayElementAtIndex(i).enumValueIndex)
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

        EditorGUILayout.PropertyField(actuationType);

        serializedObject.ApplyModifiedProperties();
    }
}