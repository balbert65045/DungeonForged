using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Path
{
    public List<PathPart> PathParts = new List<PathPart>();

    public void RemoveRoadAndAreaFromPath()
    {
        PathParts.Remove(GetNextRoad());
        PathParts.Remove(GetNextPosition());
    }
    public void RemovePath()
    {
        foreach(PathPart part in PathParts)
        {
            part.Remove();
        }
    }
    public void AddToPath(PathPart part)
    {
        PathParts.Add(part);
        part.pathOn = this;
    }

    public void HighlightPath()
    {
        foreach(PathPart part in PathParts)
        {
            part.HighlightPath();
        }
    }

    public void ReturnToNormal()
    {
        foreach (PathPart part in PathParts)
        {
            part.ReturnToNormal();
        }
    }

    public Road GetNextRoad()
    {
        for(int i = 0; i < PathParts.Count; i++)
        {
            if (PathParts[i].GetComponent<Road>() != null) { return PathParts[i].GetComponent<Road>(); }
        }
        return null;
    }

    public LocationArea GetNextPosition()
    {
        for (int i = 0; i < PathParts.Count; i++)
        {
            if (PathParts[i].GetComponent<LocationArea>() != null) { return PathParts[i].GetComponent<LocationArea>(); }
        }
        return null;
    }
}

public class LocationMap : MonoBehaviour
{
    public Hashtable Map = new Hashtable();
    LocationArea StartArea;
    public List<int> AvailableDirections = new List<int>() { 0, 1, 2, 3 };

    public GameObject PathPrefab;
    public List<Path> Paths = new List<Path>();
    public float mapSpeed = 100f;

    public void ClearPaths() { Paths.Clear(); }
    public void HideOtherPaths(Path newPath)
    {
        foreach (Path path in Paths)
        {
            if (path != newPath)
            {
                path.RemovePath();
            }
        }
        Paths.Clear();
        Paths.Add(newPath);
    }
    public void CreateTable()
    {
        Map.Clear();
        LocationArea[] areas = GetComponentsInChildren<LocationArea>();
        foreach (LocationArea area in areas)
        {
            AddArea(area);
        }
    }

    public void AddArea(LocationArea area) { Map.Add(GetAreaHash(area.X, area.Y), area); }
    public string GetAreaHash(int x, int y) { return x.ToString() + "," + y.ToString(); }
    public LocationArea GetArea(int x, int y) { return (LocationArea)Map[GetAreaHash(x, y)]; }

    void Awake()
    {
        CreateTable();
        SetStartArea();
    }

    void Start()
    {
        //Debug.Log("Start hit");
        //SetStartArea();
        //ShowPossibleOtherAreas();
    }

    public void CreatePathsIfNoPaths()
    {
        if (Paths.Count == 0)
        {
            ShowPossibleOtherAreas();
        }
    }

    public void MoveInDirection(Path path)
    {
        StartCoroutine("Move", path);
    }

    IEnumerator Move(Path path)
    {
        RoadDirection direction = path.GetNextRoad().myDirection;
        bool moving = true;
        Vector3 StartPosition = transform.position;
        while (moving)
        {
            if ((StartPosition - transform.position).magnitude > 200) { moving = false; }
            switch (direction)
            {
                case RoadDirection.Up:
                    transform.Translate(Vector3.up * Time.deltaTime * -mapSpeed);
                    break;
                case RoadDirection.Right:
                    transform.Translate(Vector3.right * Time.deltaTime * -mapSpeed);
                    break;
                case RoadDirection.Down:
                    transform.Translate(Vector3.down * Time.deltaTime * -mapSpeed);
                    break;
                case RoadDirection.Left:
                    transform.Translate(Vector3.left * Time.deltaTime * -mapSpeed);
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
        StartArea = path.GetNextPosition();
        StartArea.SetAsUsed();
        path.RemoveRoadAndAreaFromPath();
        if (path.PathParts.Count == 0) { Paths.Remove(path); }
        GetComponentInParent<MapCanvas>().HideMap();
        StartArea.ChangeLevelToLocation();
    }

    void SetStartArea()
    {
        StartArea = GetArea(4, 4);
        StartArea.SetAsEnemy();
        StartArea.SetAsUsed();
    }

    void ShowPossibleOtherAreas()
    {
        CreateAPath();
        CreateAPath();
    }

    void CreateAPath()
    {
        Path path = new Path();
        LocationArea FirstArea = SetupUtilityArea(StartArea);
        path.AddToPath(FirstArea.GetComponent<PathPart>());
        GameObject road1 = CreateTravelPath(StartArea, FirstArea);
        path.AddToPath(road1.GetComponent<PathPart>());
        LocationArea SecondArea = SetupCombatArea(FirstArea);
        path.AddToPath(SecondArea.GetComponent<PathPart>());
        GameObject road2 = CreateTravelPath(FirstArea, SecondArea);
        path.AddToPath(road2.GetComponent<PathPart>());

        Paths.Add(path);
    }

    GameObject CreateTravelPath(LocationArea startArea, LocationArea endArea)
    {
        GameObject Road = Instantiate(PathPrefab, startArea.transform);
        Vector3 differenceVector = startArea.transform.position - endArea.transform.position;
        if (differenceVector.x > 1) { 
            Road.transform.localEulerAngles = new Vector3(0, 0, -90);
            Road.transform.localPosition = new Vector3(-100, 0, 0);
            Road.GetComponent<Road>().myDirection = RoadDirection.Left;
        }
        else if(differenceVector.x < -1){ 
            Road.transform.localEulerAngles = new Vector3(0, 0, 90);
            Road.transform.localPosition = new Vector3(100, 0, 0);
            Road.GetComponent<Road>().myDirection = RoadDirection.Right;
        }
        else if (differenceVector.y > 1) { 
            Road.transform.localEulerAngles = new Vector3(0, 0, 0);
            Road.transform.localPosition = new Vector3(0, -100, 0);
            Road.GetComponent<Road>().myDirection = RoadDirection.Down;
        }
        else if (differenceVector.y < -1) { 
            Road.transform.localEulerAngles = new Vector3(0, 0, 180);
            Road.transform.localPosition = new Vector3(0, 100, 0);
            Road.GetComponent<Road>().myDirection = RoadDirection.Up;
        }
        return Road;
    }

    LocationArea SetupRestArea(LocationArea area)
    {
        area.SetAsRestArea();
        area.SetAsUnused();
        return area;
    }

    LocationArea SetupShopArea(LocationArea area)
    {
        area.SetAsShop();
        area.SetAsUnused();
        return area;
    }

    LocationArea SetupUtilityArea(LocationArea area)
    {
        LocationArea NewArea = SetupArea(area);
        if (Random.Range(0,2) == 0)
        {
            return SetupShopArea(NewArea);
        }
        else
        {
            return SetupRestArea(NewArea);
        }
    }

    LocationArea SetupCombatArea(LocationArea area)
    {
        LocationArea NewArea = SetupArea(area);
        NewArea.SetAsEnemy();
        NewArea.SetAsUnused();
        return NewArea;
    }


    LocationArea SetupArea(LocationArea area)
    {
        AvailableDirections = new List<int>() { 0, 1, 2, 3 };
        LocationArea NewArea = FindAvailableArea(area);
        return NewArea;
    }

    LocationArea FindAvailableArea(LocationArea area)
    {
        int direction = AvailableDirections[Random.Range(0, AvailableDirections.Count)];
        switch (direction)
        {
            case 0:
                if (!AreaTakenOrNull(area.X + 1, area.Y)) {
                    LocationArea newArea = GetArea(area.X + 1, area.Y);
                    if (!AreaDeadEnd(newArea)) { return newArea; }
                }
                break;
            case 1:
                if (!AreaTakenOrNull(area.X, area.Y - 1)) {
                    LocationArea newArea = GetArea(area.X, area.Y - 1);
                    if (!AreaDeadEnd(newArea)) { return newArea; }
                }
                break;
            case 2:
                if (!AreaTakenOrNull(area.X - 1, area.Y)) {
                    LocationArea newArea = GetArea(area.X - 1, area.Y);
                    if (!AreaDeadEnd(newArea)) { return newArea; }
                }
                break;
            case 3:
                if (!AreaTakenOrNull(area.X , area.Y + 1)) {
                    LocationArea newArea = GetArea(area.X, area.Y + 1);
                    if (!AreaDeadEnd(newArea)) { return newArea; }
                }
                break;
        }
        AvailableDirections.Remove(direction);
        if (AvailableDirections.Count == 0) { return null; }
        return FindAvailableArea(area);
    }

    bool AreaDeadEnd(LocationArea area)
    {
        return (AreaTakenOrNull(area.X + 1, area.Y) && AreaTakenOrNull(area.X, area.Y - 1)
            && AreaTakenOrNull(area.X - 1, area.Y) && AreaTakenOrNull(area.X, area.Y + 1));
    }

    bool AreaTakenOrNull(int X, int Y)
    {
        if (GetArea(X,Y) == null) { return true; }
        if (GetArea(X, Y).Visible) { return true; }
        return false;
    }
}
