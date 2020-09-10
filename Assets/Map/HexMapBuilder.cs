using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexMapBuilder : MonoBehaviour {

    public Material InvisibleMaterial;
    public Transform HexPrefab;
    public int map_radius = 11;

    float hexHeight = 2.0f;
    float hexWidth = 1.732f;
    public float gap = 0.0f;

    Vector3 startPos;

    public void BuildMap()
    {
        //This is used to stop it from continually increasing 
        hexHeight = 2.0f;
        hexWidth = 1.732f;
        AddGap();
        CalculateStartPos();
        CreateGrid();
    }

    public void DestroyMap()
    {
        if (GetComponent<HexMapController>().Map != null) { GetComponent<HexMapController>().Map.Clear(); }
        Hex[] hexes = GetComponentsInChildren<Hex>();
        if (hexes.Length <= 0) { return; }
        foreach (Hex hex in hexes)
        {
            DestroyImmediate(hex.gameObject);
        }

        GameObject interactionsObjects = FindObjectOfType<InteractionObjects>().gameObject;
        int count = interactionsObjects.transform.childCount;
        for (int i = 0; i < count; i ++)
        {
            DestroyImmediate(interactionsObjects.transform.GetChild(0).gameObject);
        }

        GameObject characterObjects = FindObjectOfType<CharacterHolder>().gameObject;
        int count2 = characterObjects.transform.childCount;
        for (int i = 0; i < count2; i++)
        {
            DestroyImmediate(characterObjects.transform.GetChild(0).gameObject);
        }
    }
	
	void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CalculateStartPos()
    {
        startPos = new Vector3(0, 0, 0);
    }

    void CreateGrid()
    {
        for (int q = 0; q < map_radius * 2; q++)
        {
            int q_offset = (int)Mathf.Floor(q / 2);
            for (int r = -q_offset; r < (map_radius * 1.5f) - q_offset; r++)
            {
                Transform hex = Instantiate(HexPrefab) as Transform;
                Vector2 gridPos = new Vector2(r, q);
                hex.position = CalculateWorldPos(gridPos);

                hex.SetParent(this.transform);
                hex.name = "Hex " + r + "|" + q;
                hex.GetComponent<Node>().isAvailable = false;
                hex.GetComponent<MeshRenderer>().material = InvisibleMaterial;

                hex.GetComponent<Node>().SetNode(q, r);
            }
        }
        //for (int q = -map_radius; q <= map_radius; q++)
        //{
        //    int r1 = Mathf.Max(-map_radius, -q - map_radius);
        //    int r2 = Mathf.Min(map_radius, -q + map_radius);
        //    for (int r = r1; r <= r2; r++)
        //    {
        //        Transform hex = Instantiate(HexPrefab) as Transform;
        //        Vector2 gridPos = new Vector2(r, q);
        //        hex.position = CalculateWorldPos(gridPos);

        //        hex.SetParent(this.transform);
        //        hex.name = "Hex " + r + "|" + q;
        //        hex.GetComponent<Node>().isAvailable = false;
        //        hex.GetComponent<MeshRenderer>().material = InvisibleMaterial;

        //        hex.GetComponent<Node>().SetNode(q, r);
        //    }
        //}
    }

    Vector3 CalculateWorldPos(Vector2 gridPos)
    {
        float x = gridPos.x * hexWidth;
        float y = gridPos.y * hexHeight;

        float x_ = startPos.x + x + ((gridPos.y * hexWidth) / 2);
        float z_ = startPos.z - y * .75f;

        return new Vector3(x_, 0, z_);
    } 

}
