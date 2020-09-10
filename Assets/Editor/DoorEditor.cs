using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        Door myDoor = (Door)target;
        //if (GUILayout.Button("Get Hexes In Room"))
        //{
        //    myDoor.GetHexesInRoom();
        //}

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Door"))
        {
            myDoor.BuildDoor();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Door Tiles"))
        {
            myDoor.ShowHexes();
        }
        if (GUILayout.Button("Hide Door Tiles"))
        {
            myDoor.HideHexes();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Build Door By Dimensions"))
        {
            myDoor.BuildRoomBySize();
        }
    }
}
