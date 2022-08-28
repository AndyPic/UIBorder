using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleUIBorder))]
[CanEditMultipleObjects]
public class SimpleUIBorderEditor : Editor
{
    SerializedProperty differentEdgeWidths;
    SerializedProperty differentEdgeColors;

    SerializedProperty edgeWidths;
    SerializedProperty edgeColors;

    SerializedProperty borderAlignment;
    SerializedProperty cornerStyle;
    SerializedProperty cornerExtendDistance;
    SerializedProperty cornerInsetDistance;

    SerializedProperty borderStyle;
    SerializedProperty pixelsPerUnitMultiplier;

    private void OnEnable()
    {
        differentEdgeWidths = serializedObject.FindProperty("differentEdgeWidths");
        differentEdgeColors = serializedObject.FindProperty("differentEdgeColors");

        edgeWidths = serializedObject.FindProperty("edgeWidths");
        edgeColors = serializedObject.FindProperty("edgeColors");

        borderAlignment = serializedObject.FindProperty("borderAlignment");
        cornerStyle = serializedObject.FindProperty("cornerStyle");
        cornerExtendDistance = serializedObject.FindProperty("cornerExtendDistance");
        cornerInsetDistance = serializedObject.FindProperty("cornerInsetDistance");

        borderStyle = serializedObject.FindProperty("borderStyle");
        pixelsPerUnitMultiplier = serializedObject.FindProperty("pixelsPerUnitMultiplier");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Handle edge widths
        EditorGUILayout.PropertyField(differentEdgeWidths, new GUIContent("Different Edge Widths?", "When disabled, all widths will be set to the Left width"));
        if (differentEdgeWidths.boolValue)
        {
            for (int i = 0; i < edgeWidths.arraySize; i++)
            {
                EditorGUILayout.PropertyField(edgeWidths.GetArrayElementAtIndex(i), new GUIContent($"{(SimpleUIBorder.E_BorderEdge)i} Width"));
            }
        }
        else
        {
            EditorGUILayout.PropertyField(edgeWidths.GetArrayElementAtIndex(0), new GUIContent("Width"));

            edgeWidths.GetArrayElementAtIndex(1).floatValue = edgeWidths.GetArrayElementAtIndex(0).floatValue;
            edgeWidths.GetArrayElementAtIndex(2).floatValue = edgeWidths.GetArrayElementAtIndex(0).floatValue;
            edgeWidths.GetArrayElementAtIndex(3).floatValue = edgeWidths.GetArrayElementAtIndex(0).floatValue;

        }

        EditorGUILayout.Space();

        // Handle edge colors
        EditorGUILayout.PropertyField(differentEdgeColors, new GUIContent("Different Edge Colors?", "When disabled, all colors will be set to the Left color"));
        if (differentEdgeColors.boolValue)
        {
            for (int i = 0; i < edgeColors.arraySize; i++)
            {
                EditorGUILayout.PropertyField(edgeColors.GetArrayElementAtIndex(i), new GUIContent($"{(SimpleUIBorder.E_BorderEdge)i} Color"));
            }
        }
        else
        {
            EditorGUILayout.PropertyField(edgeColors.GetArrayElementAtIndex(0), new GUIContent("Color"));

            edgeColors.GetArrayElementAtIndex(1).colorValue = edgeColors.GetArrayElementAtIndex(0).colorValue;
            edgeColors.GetArrayElementAtIndex(2).colorValue = edgeColors.GetArrayElementAtIndex(0).colorValue;
            edgeColors.GetArrayElementAtIndex(3).colorValue = edgeColors.GetArrayElementAtIndex(0).colorValue;

            (target as SimpleUIBorder).SetBorderColor(edgeColors.GetArrayElementAtIndex(0).colorValue);

        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(borderAlignment);

        EditorGUILayout.PropertyField(cornerStyle);

        if (cornerStyle.enumValueIndex == 2) // Extended
        {
        }

        switch (cornerStyle.enumValueIndex)
        {
            case 2:
                EditorGUILayout.PropertyField(cornerExtendDistance);
                break;

            case 3:
                EditorGUILayout.PropertyField(cornerInsetDistance);
                break;

            default:
                // Do nothing by default
                break;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(borderStyle);

        if (borderStyle.enumValueIndex == 1)
            EditorGUILayout.PropertyField(pixelsPerUnitMultiplier, new GUIContent("Pixels Per Unit Multiplier", "Higher number = More dashes"));

        serializedObject.ApplyModifiedProperties();
    }
}