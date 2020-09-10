using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardBuilder))]
public class CardBuilderEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CardBuilder cardBuilder = (CardBuilder)target;
        if (GUILayout.Button("Create Card"))
        {
            cardBuilder.BuildCard();
        }
    }

}
