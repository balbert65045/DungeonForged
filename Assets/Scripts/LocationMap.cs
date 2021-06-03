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
    public LocationArea StartArea;
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
        CreateFirstPath();
    }

    void Start()
    {
        //Debug.Log("Start hit");
        //SetStartArea();
        //ShowPossibleOtherAreas();
    }

    private void Update()
    {
        if (pathMovingOn != null) { return; }
        if (Input.GetKey(KeyCode.S))
        {
            transform.localPosition = Vector3.Lerp(transform.up * mapSpeed * .05f + transform.localPosition, transform.localPosition, .5f);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.localPosition = Vector3.Lerp(transform.right * mapSpeed * .05f + transform.localPosition, transform.localPosition, .5f);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition = Vector3.Lerp(transform.up * -mapSpeed * .05f + transform.localPosition, transform.localPosition, .5f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.localPosition = Vector3.Lerp(transform.right * -mapSpeed * .05f + transform.localPosition, transform.localPosition, .5f);
        }
        float x = transform.localPosition.x;
        float y = transform.localPosition.y;
        if (transform.localPosition.x > -minWidth * 150) { x = -minWidth * 150; }
        if (transform.localPosition.x < -maxWidth * 150) { x = -maxWidth * 150; }
        if (transform.localPosition.y > maxHeight * 150) { y = maxHeight * 150; }
        if (transform.localPosition.y < minHeight * 150) { y = minHeight * 150; }

        transform.localPosition = new Vector3(x, y, 0);
    }

    public int PathsTaken = 0;

    public void CreatePathsIfNoPaths()
    {
        if (Paths.Count == 0)
        {
            if (PathsTaken >= 3)
            {
                CreateBossPath();
            }
            else
            {
                ShowPossibleOtherAreas();
                PathsTaken++;
            }
        }
    }

    public void MoveInDirection(Path path)
    {
        StartCoroutine("Move", path);
    }

    bool moving = false;
    IEnumerator Move(Path path)
    {
        pathMovingOn = path;
        RoadDirection direction = path.GetNextRoad().myDirection;
        moving = true;
        transform.localPosition = new Vector3((StartArea.X-4) * -150, (StartArea.Y-4) * 150, 0);
        Vector3 StartPosition = transform.localPosition;
        while (moving)
        {
            if ((StartPosition - transform.localPosition).magnitude > 150) { moving = false; }
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
    }

    Path pathMovingOn;

    public void ChangeToNextLocation()
    {
        StartArea = pathMovingOn.GetNextPosition();
        StartArea.SetAsUsed();
        pathMovingOn.RemoveRoadAndAreaFromPath();
        if (pathMovingOn.PathParts.Count == 0) { Paths.Remove(pathMovingOn); }
        StartArea.ChangeLevelToLocation();
        GetComponentInParent<MapCanvas>().HideMapWithDelay();
        pathMovingOn = null;
    }

    void SetStartArea()
    {
        StartArea = GetArea(4, 4);
        StartArea.SetAsStart();
        StartArea.SetAsUsed();
    }

    void CreateBossPath()
    {
        Path path = new Path();
        LocationArea FirstArea = SetupRestArea(StartArea);
        path.AddToPath(FirstArea.GetComponent<PathPart>());
        GameObject road1 = CreateTravelPath(StartArea, FirstArea);
        path.AddToPath(road1.GetComponent<PathPart>());
        LocationArea SecondArea = SetUpBossArea(FirstArea);
        path.AddToPath(SecondArea.GetComponent<PathPart>());
        GameObject road2 = CreateTravelPath(FirstArea, SecondArea);
        path.AddToPath(road2.GetComponent<PathPart>());

        Paths.Add(path);
    }

    void CreateFirstPath()
    {
        Path path = new Path();
        LocationArea NewArea = SetupArea(StartArea);
        LocationArea FirstArea = SetupArtifactArea(NewArea);
        //LocationArea FirstArea = SetupRestArea(NewArea);
        path.AddToPath(FirstArea.GetComponent<PathPart>());
        GameObject road1 = CreateTravelPath(StartArea, FirstArea);
        path.AddToPath(road1.GetComponent<PathPart>());
        LocationArea SecondArea = SetupCombatArea(FirstArea);
        //LocationArea SecondArea = SetUpBossArea(FirstArea);
        path.AddToPath(SecondArea.GetComponent<PathPart>());
        GameObject road2 = CreateTravelPath(FirstArea, SecondArea);
        path.AddToPath(road2.GetComponent<PathPart>());
        //LocationArea ThirdArea = SetupCombatArea(SecondArea);
        //path.AddToPath(ThirdArea.GetComponent<PathPart>());
        //GameObject road3 = CreateTravelPath(SecondArea, ThirdArea);
        //path.AddToPath(road3.GetComponent<PathPart>());

        Paths.Add(path);
    }

    void ShowPossibleOtherAreas()
    {
        ResetStops();
        CreateAPath();
        CreateAPath();
    }

    List<Location> StopsAvailable = new List<Location> { Location.Anvil, Location.Artifact, Location.Chest, Location.Shop, Location.Rest, Location.Furnace }; 
    void ResetStops()
    {
        StopsAvailable = new List<Location> { Location.Anvil, Location.Artifact, Location.Chest, Location.Shop, Location.Rest, Location.Furnace };
    }

    void CreateAPath()
    {
        Path path = new Path();
        LocationArea FirstArea = SetupArea(StartArea);
        FirstArea = SetupStop(FirstArea);
        path.AddToPath(FirstArea.GetComponent<PathPart>());
        GameObject road1 = CreateTravelPath(StartArea, FirstArea);
        path.AddToPath(road1.GetComponent<PathPart>());
        LocationArea SecondArea = SetupArea(FirstArea);
        SecondArea = SetupStop(SecondArea);
        path.AddToPath(SecondArea.GetComponent<PathPart>());
        GameObject road2 = CreateTravelPath(FirstArea, SecondArea);
        path.AddToPath(road2.GetComponent<PathPart>());
        LocationArea ThirdArea = SetupCombatArea(SecondArea);
        path.AddToPath(ThirdArea.GetComponent<PathPart>());
        GameObject road3 = CreateTravelPath(SecondArea, ThirdArea);
        path.AddToPath(road3.GetComponent<PathPart>());

        Paths.Add(path);
    }

    LocationArea SetupStop(LocationArea area)
    {
        LocationArea newStop = null;
        int RandomIndex = Random.Range(0, StopsAvailable.Count);
        switch (StopsAvailable[RandomIndex])
        {
            case Location.Anvil:
                newStop = SetupAnvilArea(area);
                break;
            case Location.Artifact:
                newStop = SetupArtifactArea(area);
                break;
            case Location.Chest:
                newStop = SetupChestArea(area);
                break;
            case Location.Shop:
                newStop = SetupShopArea(area);
                break;
            case Location.Rest:
                newStop = SetupRestArea(area);
                break;
            case Location.Furnace:
                newStop = SetupFurnaceArea(area);
                break;

        }
        StopsAvailable.RemoveAt(RandomIndex);
        return newStop;

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

    LocationArea SetUpBossArea(LocationArea area)
    {
        LocationArea NewArea = SetupArea(area);
        NewArea.SetAsBoss();
        NewArea.SetAsUnused();
        return NewArea;
    }
    LocationArea SetupArtifactArea(LocationArea area)
    {
        area.SetAsArtifactArea();
        area.SetAsUnused();
        return area;
    }

    LocationArea SetupChestArea(LocationArea area)
    {
        area.SetAsChest();
        area.SetAsUnused();
        return area;
    }

    LocationArea SetupAnvilArea(LocationArea area)
    {
        area.SetAsAnvil();
        area.SetAsUnused();
        return area;
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

    LocationArea SetupFurnaceArea(LocationArea area)
    {
        area.SetAsFurnace();
        area.SetAsUnused();
        return area;
    }

    LocationArea SetupUtilityArea(LocationArea area)
    {
        LocationArea NewArea = SetupArea(area);
        int random_index = Random.Range(0, 3);
        if (random_index == 0)
        {
            return SetupShopArea(NewArea);
        }
        else if (random_index == 1)
        {
            return SetupRestArea(NewArea);
        }
        else
        {
            return SetupFurnaceArea(NewArea);
        }
    }

    LocationArea SetupCombatArea(LocationArea area)
    {
        LocationArea NewArea = SetupArea(area);
        NewArea.SetAsEnemy();
        NewArea.SetAsUnused();
        return NewArea;
    }

    public int maxHeight = 0;
    public int maxWidth = 0;
    public int minHeight = 0;
    public int minWidth = 0;

    LocationArea SetupArea(LocationArea area)
    {
        AvailableDirections = new List<int>() { 0, 1, 2, 3 };
        LocationArea NewArea = FindAvailableArea(area);
        if (NewArea.X - 4 > maxWidth) { maxWidth = NewArea.X - 4; }
        if (NewArea.X - 4 < minWidth) { minWidth = NewArea.X - 4; }
        if (NewArea.Y - 4 > maxHeight) { maxHeight = NewArea.Y - 4; }
        if (NewArea.Y - 4 < minHeight) { minHeight = NewArea.Y - 4; }
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
