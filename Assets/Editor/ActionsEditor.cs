using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Action))]
public class ActionsEditor : Editor
{

    public SerializedProperty
        ActionType_Property,
        Amount_Property;

    Action action;

    private void OnEnable()
    {
        ActionType_Property = serializedObject.FindProperty("thisActionType");
        Amount_Property = serializedObject.FindProperty("Amount");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(ActionType_Property);

        ActionType actionType = (ActionType)ActionType_Property.enumValueIndex + 1;

        switch (actionType)
        {
            case ActionType.Attack:
                EditorGUILayout.PropertyField(Amount_Property, new GUIContent("Attack Modifier"));
                break;
            case ActionType.Movement:
                EditorGUILayout.PropertyField(Amount_Property, new GUIContent("Move Modifier"));
                break;
            case ActionType.Heal:
                EditorGUILayout.PropertyField(Amount_Property, new GUIContent("Heal Modifier"));
                break;
            case ActionType.Shield:
                EditorGUILayout.PropertyField(Amount_Property, new GUIContent("Shield Modifier"));
                break;
        }

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
    }
}
