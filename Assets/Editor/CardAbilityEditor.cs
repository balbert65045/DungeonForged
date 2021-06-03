using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardAbility))]
public class CombatCardAbilityEditor : Editor
{
    public SerializedProperty
        Staging_Property,
        Lost_Property,
        AbilityType_Property,
        Actions_Property,
        ActionsCount_Property;

    CardAbility cardAbility;

    private void OnEnable()
    {
        Staging_Property = serializedObject.FindProperty("Staging");
        Lost_Property = serializedObject.FindProperty("LostAbility");
        Actions_Property = serializedObject.FindProperty("Actions");
        ActionsCount_Property = serializedObject.FindProperty("Actions.Array.size");
        cardAbility = (CardAbility)target;
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(Staging_Property);
        EditorGUILayout.PropertyField(Lost_Property);
        //EditorGUILayout.PropertyField(Actions_Property, new GUIContent("Actions Field"), false);
        EditorGUILayout.BeginHorizontal();
        int OldArrayNumber = ActionsCount_Property.intValue;
        EditorGUILayout.LabelField("Number of Actions");
        int ArrayNumber = EditorGUILayout.IntField(OldArrayNumber);
        if (ArrayNumber != OldArrayNumber)
            ActionsCount_Property.intValue = ArrayNumber;
        EditorGUILayout.EndHorizontal();

        //ActionType actionType = (ActionType)Actions_Property.enumValueIndex + 1;
        for (int i = 1; i <= Actions_Property.arraySize; i++)
        {
            SerializedProperty ActionType_Property = Actions_Property.GetArrayElementAtIndex(i-1).FindPropertyRelative("thisActionType");
            EditorGUILayout.PropertyField(ActionType_Property, new GUIContent("Action " + i));
            ActionType actionType = (ActionType)ActionType_Property.enumValueIndex + 1;
            switch (actionType)
            {
                case ActionType.Attack:
                    SerializedProperty ActionAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionAOEType_Property = ActionAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionAOEType_Property);

                    SerializedProperty ActionAttackRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionAttackRange_Property);

                    SerializedProperty ActionAttackAmount_Property = ActionAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionAttackAmount_Property);

                    SerializedProperty ActionStatus_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisDeBuff");
                    EditorGUILayout.PropertyField(ActionStatus_Property);

                    SerializedProperty ActionTrigger_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("TriggerAnimation");
                    EditorGUILayout.PropertyField(ActionTrigger_Property);

                    break;
                case ActionType.Movement:
                    SerializedProperty ActionMovementAmount_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionMovementAmount_Property);
                    break;
                case ActionType.Heal:

                    SerializedProperty ActionHealAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionHealAOEType_Property = ActionHealAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionHealAOEType_Property);

                    SerializedProperty ActionHealRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionHealRange_Property);

                    SerializedProperty ActionHealAmount_Property = ActionHealAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionHealAmount_Property);

                    SerializedProperty ActionHealStatus_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisDeBuff");
                    EditorGUILayout.PropertyField(ActionHealStatus_Property);
                    break;
                case ActionType.Shield:
                    SerializedProperty ActionShieldAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionShieldAOEType_Property = ActionShieldAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionShieldAOEType_Property);

                    SerializedProperty ActionShieldRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionShieldRange_Property);

                    SerializedProperty ActionShieldAmount_Property = ActionShieldAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionShieldAmount_Property);

                    SerializedProperty ActionShieldStatus_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisDeBuff");
                    EditorGUILayout.PropertyField(ActionShieldStatus_Property);
                    break;
                case ActionType.BuffAttack:
                    SerializedProperty ActionBuffAttackAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionBuffAttackAOEType_Property = ActionBuffAttackAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionBuffAttackAOEType_Property);

                    SerializedProperty ActionBuffAttackRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionBuffAttackRange_Property);

                    SerializedProperty ActionBuffAttackAOEdAmount_Property = ActionBuffAttackAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionBuffAttackAOEdAmount_Property);

                    SerializedProperty ActionBuffAttackDuration_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Duration");
                    EditorGUILayout.PropertyField(ActionBuffAttackDuration_Property);
                    break;
                case ActionType.BuffMove:
                    SerializedProperty ActionBuffMoveAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionBuffMoveAOEType_Property = ActionBuffMoveAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionBuffMoveAOEType_Property);

                    SerializedProperty ActionBuffMoveRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionBuffMoveRange_Property);

                    SerializedProperty ActionBuffMoveAOEdAmount_Property = ActionBuffMoveAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionBuffMoveAOEdAmount_Property);

                    SerializedProperty ActionBuffMoveDuration_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Duration");
                    EditorGUILayout.PropertyField(ActionBuffMoveDuration_Property);
                    break;
                case ActionType.BuffRange:
                    SerializedProperty ActionBuffRangeAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionBuffRangeAOEType_Property = ActionBuffRangeAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionBuffRangeAOEType_Property);

                    SerializedProperty ActionBuffRangeRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionBuffRangeRange_Property);

                    SerializedProperty ActionBuffRangeAOEdAmount_Property = ActionBuffRangeAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionBuffRangeAOEdAmount_Property);

                    SerializedProperty ActionBuffRangeDuration_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Duration");
                    EditorGUILayout.PropertyField(ActionBuffRangeDuration_Property);
                    break;
                case ActionType.BuffArmor:
                    SerializedProperty ActionBuffAmorAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionBuffAmorAOEType_Property = ActionBuffAmorAOE_Property.FindPropertyRelative("thisAOEType");
                    EditorGUILayout.PropertyField(ActionBuffAmorAOEType_Property);

                    SerializedProperty ActionBuffArmorRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionBuffArmorRange_Property);

                    SerializedProperty ActionBuffAmorAOEdAmount_Property = ActionBuffAmorAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionBuffAmorAOEdAmount_Property);

                    SerializedProperty ActionBuffAmorDuration_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Duration");
                    EditorGUILayout.PropertyField(ActionBuffAmorDuration_Property);
                    break;
                case ActionType.Scout:
                    SerializedProperty ActionScoutRange_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Range");
                    EditorGUILayout.PropertyField(ActionScoutRange_Property);
                    break;
                case ActionType.Stealth:
                    SerializedProperty ActionStealthDuration_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("Duration");
                    EditorGUILayout.PropertyField(ActionStealthDuration_Property);
                    break;
                case ActionType.LoseHealth:
                    SerializedProperty ActionLoseHealthAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionLoseHealthAOEAmount_Property = ActionLoseHealthAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionLoseHealthAOEAmount_Property);
                    break;
                case ActionType.DrawCard:
                    SerializedProperty ActionDrawCardAOE_Property = Actions_Property.GetArrayElementAtIndex(i - 1).FindPropertyRelative("thisAOE");
                    SerializedProperty ActionDrawCardAOEAmount_Property = ActionDrawCardAOE_Property.FindPropertyRelative("Damage");
                    EditorGUILayout.PropertyField(ActionDrawCardAOEAmount_Property);
                    break;

            }
        }



        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();

    }
}
