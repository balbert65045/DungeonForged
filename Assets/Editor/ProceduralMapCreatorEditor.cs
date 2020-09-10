using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralMapCreator))]
public class ProceduralMapCreatorEditor : Editor {


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ProceduralMapCreator proceduralMapCreator = (ProceduralMapCreator)target;
        if (GUILayout.Button("Create Map"))
        {
            proceduralMapCreator.BuildMap();
        }
        if (GUILayout.Button("Build New Map"))
        {
            proceduralMapCreator.BuildNewMap();
        }
        if (GUILayout.Button("Build New Map And Show"))
        {
            proceduralMapCreator.BuildNewMapAndReveal();
        }
    }
}
