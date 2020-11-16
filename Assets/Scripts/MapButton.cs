using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButton : MonoBehaviour
{
    bool Showing = false;
   public void ToggleMap()
    {
        Showing = !Showing;
        if (Showing)
        {
            FindObjectOfType<MapCanvas>().ShowMap();
        }
        else
        {
            FindObjectOfType<MapCanvas>().HideMap();
        }
    }
}
