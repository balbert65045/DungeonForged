using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexBuilderCube))]
public class MapEditor2 : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexBuilderCube myMapBuilder = (HexBuilderCube)target;
        if (GUILayout.Button("Build Map"))
        {
            myMapBuilder.BuildMap();
        }

        if (GUILayout.Button("Destroy Map"))
        {
            myMapBuilder.DestroyMap();
        }
    }

}
