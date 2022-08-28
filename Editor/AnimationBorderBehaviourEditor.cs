using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationBorderBehaviour))]
[CanEditMultipleObjects]
public class AnimationBorderBehaviourEditor : Editor
{
    SerializedProperty animationType;
    SerializedProperty rotationDirection;
    SerializedProperty endColor;
    SerializedProperty endWidth;
    SerializedProperty animationDuration;

    private void OnEnable()
    {
        animationType = serializedObject.FindProperty("animationType");
        rotationDirection = serializedObject.FindProperty("rotationDirection");
        endColor = serializedObject.FindProperty("endColor");
        endWidth = serializedObject.FindProperty("endWidth");
        animationDuration = serializedObject.FindProperty("animationDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(animationType);

        switch (animationType.enumValueIndex)
        {
            case 0: // Flash
                EditorGUILayout.PropertyField(endColor, new GUIContent("End Color", "Color will 'flash' from current color, to this color."));
                break;

            case 1: // Pulse
                EditorGUILayout.PropertyField(endWidth, new GUIContent("End Width", "Width will 'pulse' from current width, to this width."));
                break;

            case 2: // Rotate
                EditorGUILayout.PropertyField(rotationDirection);
                break;

        }

        EditorGUILayout.PropertyField(animationDuration, new GUIContent("Animation Duration", "The time (s) to do 1 full animation cycle."));


        serializedObject.ApplyModifiedProperties();
    }
}