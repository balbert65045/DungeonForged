using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexMapBuilder))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexMapBuilder myMapBuilder = (HexMapBuilder)target;
        if (GUILayout.Button("Build Map"))
        {
            myMapBuilder.BuildMap();
        }

        if(GUILayout.Button("Destroy Map"))
        {
            myMapBuilder.DestroyMap();
        }
    }

}
