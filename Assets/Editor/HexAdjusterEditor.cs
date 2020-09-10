using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexAdjuster)), CanEditMultipleObjects]
public class HexAdjusterEditor : Editor {

	public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexAdjuster hexAdjuster = (HexAdjuster)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Full Hex"))
        {
            hexAdjuster.SetHexToFull();
        }
        if (GUILayout.Button("Half Hex"))
        {
            hexAdjuster.SetHexToHalf();
        }
        if (GUILayout.Button("Fragment Hex"))
        {
            hexAdjuster.SetHexToFragment();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Double Across"))
        {
            hexAdjuster.SetHexToDoubleAcross();
        }
        if (GUILayout.Button("Double Side"))
        {
            hexAdjuster.SetHexToDoubleSide();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+ 60"))
        {
            hexAdjuster.Rotate60Forward();
        }
        if (GUILayout.Button("180"))
        {
            hexAdjuster.Rotate180();
        }
        if (GUILayout.Button("- 60"))
        {
            hexAdjuster.Rotate60Backward();
        }
        GUILayout.EndHorizontal();

    }
}
