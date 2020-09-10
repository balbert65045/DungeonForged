using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexWallAdjuster))]
public class HexWallAdjusterEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HexWallAdjuster hexWallAdjuster = (HexWallAdjuster)target;
        if (GUILayout.Button("Build Left Wall"))
        {
            hexWallAdjuster.CreateHexWall(0, 0, "A");
        }
        if (GUILayout.Button("Build Right Wall"))
        {
            hexWallAdjuster.CreateHexWall(0, 1, "A");
        }
        if (GUILayout.Button("Build Half Wall"))
        {
            hexWallAdjuster.CreateHexWall(1, 0, "A");
        }
        if (GUILayout.Button("Build Corner Wall"))
        {
            hexWallAdjuster.CreateHexWall(2, 0, "A");
        }
    }
}
