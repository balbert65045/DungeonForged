using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMapController : MonoBehaviour
{
    public bool Moving = false;

    public void MoveOnPath(Path path)
    {
        Moving = true;
        FindObjectOfType<LocationMap>().HideOtherPaths(path);
        path.GetNextRoad().MoveOnPath();
        FindObjectOfType<LocationMap>().MoveInDirection(path);
    }

    private void Start()
    {
        //FindObjectOfType<MapCanvas>().HideMapButton();
        FindObjectOfType<MapCanvas>().ShowMap();
        FindObjectOfType<LocationMap>().CreatePathsIfNoPaths();
    }

    private void Update()
    {
        
    }
}
