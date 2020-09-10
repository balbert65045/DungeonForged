using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AxialMap {
    public AxialHex[,] Map;
     
    public AxialMap(int length, int width)
    {
        Map = new AxialHex[length, width];
    }

    public void AddHex(AxialHex hex, int x, int y)
    {
        Map[x, y] = hex;
    }
}
