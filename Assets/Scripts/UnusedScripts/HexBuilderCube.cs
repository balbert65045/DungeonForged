using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBuilderCube : MonoBehaviour {

    public Transform HexPrefab;

    public int gridHeight = 11;
    public int gridWidth = 11;
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
        GetComponent<AxialHexController>().HexMap.Clear();
        Hex[] hexes = GetComponentsInChildren<Hex>();
        if (hexes.Length <= 0) { return; }
        foreach (Hex hex in hexes)
        {
            DestroyImmediate(hex.gameObject);
        }
    }

    void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CalculateStartPos()
    {
        float offset = 0;
        if (gridHeight / 2 % 2 != 0)
        {
            offset = hexWidth / 2;
        }

        float x = hexWidth * (gridWidth / 2);
        float z = hexHeight * .75f * (gridHeight / 2);

        startPos = new Vector3(x, 0, z);
    }

    void CreateGrid()
    {
        for (int q= -map_radius; q <= map_radius; q++)
        {
            int r1 = Mathf.Max(-map_radius, -q - map_radius);
            int r2 = Mathf.Min(map_radius, -q + map_radius);
            for (int r = r1; r <= r2; r++)
            {
                Transform hex = Instantiate(HexPrefab) as Transform;
                Vector2 gridPos = new Vector2(r, q);
                hex.position = CalculateWorldPos(gridPos);

                hex.SetParent(this.transform);
                hex.name = "Hex " + r + "|" + q;

                hex.GetComponent<Node2>().SetNode(r, q);
                GetComponent<AxialHexController>().AddHex(hex.GetComponent<Node2>());
            }
        }
        AxialHexController hexController = GetComponent<AxialHexController>();
        Debug.Log(hexController.HexDistance(hexController.GetNode(0, 0), hexController.GetNode(2, 2)));
    }

    Vector3 CalculateWorldPos(Vector2 gridPos)
    {
        float x = gridPos.x * hexWidth;
        float y = gridPos.y * hexHeight;

        float x_ = startPos.x + x + ((gridPos.y * hexWidth)/ 2);
        float z_ = startPos.z - y * .75f;

        return new Vector3(x_, 0, z_);
    }

    Vector3 oddr_to_cube(Vector2 point) {
        int x = (int)point.x - ((int)point.y - ((int)point.y & 1)) / 2;
        int z = (int)point.y;
        int y = -x - z;
        return new Vector3(x, y, z);
    }

    Vector2 cube_to_axial(Vector3 cube) {
        int q = (int)cube.x;
        int r = (int)cube.z;
        return new Vector2(q, r);
    }

    Vector3 axial_to_cube(int q, int r)
    {
        int x = q;
        int z = r;
        int y = -x - z;
        return new Vector3(x, y, z);
    }


}
