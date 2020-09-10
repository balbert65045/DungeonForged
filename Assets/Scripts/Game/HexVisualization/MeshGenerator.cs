using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {

    public Character characterLinkedTo = null;
    public Material MovementMaterial;
    public Material AttackMaterial;
    public Material ArmorMaterial;
    public Material HealMaterial;

    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public void SetCurrentMaterial(ActionType type)
    {
        switch (type)
        {
            case ActionType.Movement:
                GetComponent<MeshRenderer>().material = MovementMaterial;
                break;
            case ActionType.Attack:
                GetComponent<MeshRenderer>().material = AttackMaterial;
                break;
            case ActionType.Shield:
                GetComponent<MeshRenderer>().material = ArmorMaterial;
                break;
            case ActionType.Heal:
                GetComponent<MeshRenderer>().material = HealMaterial;
                break;
            default:
                GetComponent<MeshRenderer>().material = HealMaterial;
                break;
        }
    }

    public void DeleteMesh()
    {
        if (mesh == null) { return; }
        mesh.Clear();
    }

	// Use this for initialization
	public void CreateMesh (List<Vector3> newPoints) {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape(newPoints);
        UpdateMesh();
	}

    void CreateShape(List<Vector3> newPoints)
    {

        List<Vector2> t = new List<Vector2>();
        for(int i = 0; i < newPoints.Count; i++)
        {
            t.Add(new Vector2(newPoints[i].x, newPoints[i].z));
        }

        Triangulator tr = new Triangulator(t.ToArray());
        int[] indices = tr.Triangulate();

        vertices = newPoints.ToArray();
        triangles = indices;
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

	
}
