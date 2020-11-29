using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EdgeHexes
{
    public string Room;
    public List<Hex> Edges = new List<Hex>();
    public void AddHex(Hex hex) { Edges.Add(hex); }
    public void RemoveHex(Hex hex) { Edges.Remove(hex); }
}

class Rooms
{
    public string RoomName = "";
    public List<Hex> RoomHexes = new List<Hex>();
    public void AddHex(Hex hex) { RoomHexes.Add(hex); }
    public void RemoveHex(Hex hex) { RoomHexes.Remove(hex); }
}

public class ProceduralMapCreator : MonoBehaviour {

    public List<GameObject> RoomDatabase = new List<GameObject>();
    public List<GameObject> RoomsWithWallsNotInDirection(RoomSide side)
    {
        List<GameObject> RoomsAvailable = new List<GameObject>();
        foreach (GameObject room in RoomDatabase)
        {
            if (room.GetComponent<Room>().DoorOpenings.Count == 0) { continue; }
            if (room.GetComponent<Room>().DoorOpenings.Contains(side)) { continue; }
            RoomsAvailable.Add(room);
        }
        return RoomsAvailable;
    }
    //public List<GameObject> DeadEndRooms()
    //{
    //    List<GameObject> RoomsAvailable = new List<GameObject>();
    //    foreach (GameObject room in RoomDatabase)
    //    {
    //        if (room.GetComponent<Room>().DoorOpenings.Count == 0) { RoomsAvailable.Add(room); }
    //    }
    //    return RoomsAvailable;
    //}

    List<Door> doorsAvailable = new List<Door>();

    public int GoldAmount = 50;
    public int CurrentGoldAmount = 0;

    public int TreasureAmount = 7;
    public int CurrentTreasureAmount = 0;

    public int ChallengeRating = 15;
    public int CurrentChallengeRating = 0;
    public int MaxLength = 15;
    public int MaxRoomNumber = 10;
    public GameObject ExplorationChestPrefab;
    public GameObject CombatChestPrefab;
    public List<GameObject> ObstaclePool = new List<GameObject>();
    public List<GameObject> EnemyPool = new List<GameObject>();
    public List<GameObject> PlayerCharacters = new List<GameObject>();

    HexMapController hexMap;
    HexRoomBuilder hexRoomBuilder;
    private int RoomIndex = 0;

    List<EdgeHexes> EdgesAvailable;
    List<Rooms> RoomsMade;

    int totalEnemiesOut = 0;

    //Create a random room size

    //Add Enemies dependent on size, enemies that have already been added, and randomness
    //With a max enemy for the room
    //Randomely select a hex to place the enemy on

    //Add Obstacles dependent on room size, max obstacles for the room and randomness
    //Select random hexes for the room.

    // Add a door either 1 or 3 dependent on randomness

    //repeat
    bool ShowMap = false;

    public void BuildNewMapAndReveal()
    {
        ShowMap = true;
        FindObjectOfType<HexMapBuilder>().DestroyMap();
        FindObjectOfType<HexMapBuilder>().BuildMap();
        BuildMap();
    }

    public void BuildNewMap()
    {
        ShowMap = false;
        FindObjectOfType<HexMapBuilder>().DestroyMap();
        FindObjectOfType<HexMapBuilder>().BuildMap();
        BuildMap();
    }

    public void BuildMapToStart(List<GameObject> Characters)
    {
        PlayerCharacters = Characters;
        ShowMap = false;
        FindObjectOfType<HexMapBuilder>().DestroyMap();
        FindObjectOfType<HexMapBuilder>().BuildMap();
        BuildMap();
    }

    public void BuildMap()
    {

        totalEnemiesOut = 0;
        CurrentGoldAmount = 0;
        RoomIndex = 0;
        attempts = 0;
        DestroyAllRooms();
        CurrentTreasureAmount = TreasureAmount;
        EdgesAvailable = new List<EdgeHexes>();
        RoomsMade = new List<Rooms>();
        doorsAvailable = new List<Door>();

        CurrentChallengeRating = ChallengeRating;
        hexMap = FindObjectOfType<HexMapController>();
        hexMap.CreateTable();
        hexRoomBuilder = FindObjectOfType<HexRoomBuilder>();
        List<Hex> StartHexes = CreateStartRoom();
        //CollectAndSortEdges(StartHexes, new List<Hex>(), "A");
        if (StartHexes.Count == 0) { return; }
        CreateNextRoom();
        HideAllDoorsNotUsed();
        HideRooms();
        //PopulateRooms();
        FindObjectOfType<ObjectiveArea>().SetTotalEnemies(totalEnemiesOut);
        //Create exit
        //CreateExit();
    }

    void HideAllDoorsNotUsed()
    {
        foreach(Door door in doorsAvailable)
        {
            door.door.GetComponent<DoorNew>().TurnDoorToRailing();
        }
    }

    void DestroyAllRooms()
    {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach(Room room in rooms)
        {
            DestroyImmediate(room.gameObject);
        }
    }

    void HideRooms()
    {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach(Room room in rooms)
        {
            if (!room.start)
            {
                room.gameObject.SetActive(false);
            }
        }
    }

    void PopulateRooms()
    {
        int RoomsWithEnemiesInside = CurrentChallengeRating / 3;
        List<Rooms> RoomToBuildEnemies = new List<Rooms>();
        foreach(Rooms room in RoomsMade) { RoomToBuildEnemies.Add(room); }
        for (int i = 0; i < RoomsWithEnemiesInside; i++)
        {
            int RandomIndex = Random.Range(0, RoomToBuildEnemies.Count);
            AddEnemies(RoomToBuildEnemies[RandomIndex].RoomHexes);
            RoomToBuildEnemies.Remove(RoomToBuildEnemies[RandomIndex]);
        }

        foreach (Rooms room in RoomsMade) { AddObstaclesToRoom(room.RoomHexes); }

        int totalRooms = RoomsMade.Count;
        List<Rooms> RoomToBuildChests= new List<Rooms>();
        foreach (Rooms room in RoomsMade) { RoomToBuildChests.Add(room); }
        for (int i = 0; i < totalRooms; i++)
        {
            int RandomIndex = Random.Range(0, RoomToBuildChests.Count);
            AddChestsToRoom(RoomToBuildChests[RandomIndex].RoomHexes);
            RoomToBuildChests.Remove(RoomToBuildChests[RandomIndex]);
        }

        //foreach (Rooms room in RoomsMade) { AddGoldToRooms(room.RoomHexes); }

        if (ShowMap)
        {
            // TO Show All Rooms
            foreach (Rooms room in RoomsMade)
            {
                ShowHexSet(room.RoomHexes, room.RoomName);
            }
        }
    }

    void PopulateRoom(List<Hex> hexes)
    {
        AddEnemies(hexes);
        AddObstaclesToRoom(hexes);
        AddGoldToRooms(hexes);
    }

    void AddGoldToRooms(List<Hex> hexes)
    {
        if (CurrentGoldAmount > GoldAmount) { return; }
        List<Hex> NonEdgeHexes = GetNonEdgeHexes(hexes);
        int MaximumGoldShouldPlace = 1;
        for (int i = 0; i < MaximumGoldShouldPlace; i++)
        {
            int RandomLocation = Random.Range(0, NonEdgeHexes.Count);
            Hex AttemptHex = NonEdgeHexes[RandomLocation];
            if (AttemptHex.EntityToSpawn == null)
            {
                int goldPlacing = Random.Range(3, 15);
                CurrentGoldAmount += goldPlacing;
                AttemptHex.goldHolding = goldPlacing;
            }
        }
    }

    void RemoveAllHexesFromRoom(string roomName)
    {
        for(int i = 0; i < EdgesAvailable.Count; i++)
        {
            if (EdgesAvailable[i].Room == roomName) { EdgesAvailable.Remove(EdgesAvailable[i]); }
        }
    }

    void CollectAndSortEdges(List<Hex> hexes, List<Hex> EdgeFrom, string roomName)
    {
        List<Hex> edgeHexes = GetEdgeHexes(hexes);
        EdgeHexes UpHex = new EdgeHexes();
        UpHex.Room = roomName;
        EdgeHexes RightHex = new EdgeHexes();
        RightHex.Room = roomName;
        EdgeHexes DownHex = new EdgeHexes();
        DownHex.Room = roomName;
        EdgeHexes LeftHex = new EdgeHexes();
        LeftHex.Room = roomName;
        int i = 0;
        while (i < edgeHexes.Count)
        {
            if (EdgeFrom.Contains(edgeHexes[i]))
            {
                edgeHexes.Remove(edgeHexes[i]);
            }
            else
            {
                i++;
            }
        }
        foreach (Hex hex in edgeHexes)
        {
            if (hex.GetComponent<HexAdjuster>().DownRoomSide != "") { UpHex.AddHex(hex); }
            else if (hex.GetComponent<HexAdjuster>().LeftRoomSide != "") { RightHex.AddHex(hex); }
            else if (hex.GetComponent<HexAdjuster>().UpRoomSide != "") { DownHex.AddHex(hex); }
            else if (hex.GetComponent<HexAdjuster>().RightRoomSide != "") { LeftHex.AddHex(hex); }
        }
        if (UpHex.Edges.Count > 0) { EdgesAvailable.Add(UpHex); }
        if (RightHex.Edges.Count > 0) { EdgesAvailable.Add(RightHex); }
        if (DownHex.Edges.Count > 0) { EdgesAvailable.Add(DownHex); }
        if (LeftHex.Edges.Count > 0) { EdgesAvailable.Add(LeftHex); }
    }

    List<Hex> CreateStartRoom()
    {
        int maxLength = MaxLength;
        MaxLength = 6;
        List<Hex> hexes = BuildStartRoom(25, 23, RoomSide.Right);
        MaxLength = maxLength;
        if (hexes.Count == 0) { Debug.LogWarning("Room not made!!"); }
        string RoomName = ((char)((int)('A') + RoomIndex)).ToString();
        Node StartNode = hexMap.GetNode(25, 23);
        foreach(Hex hex in hexes) { hex.GetComponent<Node>().Shown = true; }
        List<Hex> playerHexes = AddPlayerCharactersToRoom();
        List<Hex> EnemyHexes = new List<Hex>();
        foreach(Hex hex in hexes)
        {
            if (!playerHexes.Contains(hex)) { EnemyHexes.Add(hex); }
        }
        AddEnemies(EnemyHexes);
        AddObstaclesToRoom(hexes);
        AddGoldToRooms(hexes);
        SetStartNode(StartNode, RoomName);
        hexes.Add(StartNode.GetComponent<Hex>());
        ShowHexSet(hexes, RoomName);
        RoomIndex++;
        return hexes;
    }

    void AddChestsToStartHexes()
    {
        int index = 0;
        foreach (GameObject character in PlayerCharacters)
        {
            Node SpawnNode = null;
            if (index == 0)
            {
                SpawnNode = hexMap.GetNode(-1, 2);
            }
            else if (index == 1)
            {
                SpawnNode = hexMap.GetNode(-1, 3);
            }
            else if (index == 2)
            {
                SpawnNode = hexMap.GetNode(-2, 3);
            }
            else if (index == 3)
            {
                SpawnNode = hexMap.GetNode(-2, 4);
            }
            if (SpawnNode == null) { continue; }
            SpawnNode.GetComponent<Hex>().chestFor = character.GetComponent<PlayerCharacter>().CharacterName;
            SpawnNode.GetComponent<Hex>().EntityToSpawn = CombatChestPrefab.GetComponent<Entity>();
            index++;
        }
    }

    List<Hex> AddPlayerCharactersToRoom()
    {
        int index = 0;
        List<Hex> playerHexes = new List<Hex>();
        foreach (GameObject character in PlayerCharacters)
        {
            Node SpawnNode = null;
            if (index == 0)
            {
                SpawnNode = hexMap.GetNode(24, 23);
            }
            else if (index == 1)
            {
                SpawnNode = hexMap.GetNode(25, 22);
            }
            else if (index == 2)
            {
                SpawnNode = hexMap.GetNode(24, 22);
            }
            else if (index == 3)
            {
                SpawnNode = hexMap.GetNode(25, 21);
            }
            if (SpawnNode == null) { continue; }
            playerHexes.Add(SpawnNode.GetComponent<Hex>());
            Node[] adjacentOnes = hexMap.GetNeighbors(SpawnNode);
            foreach(Node node in adjacentOnes)
            {
                if (!playerHexes.Contains(node.GetComponent<Hex>())) { playerHexes.Add(node.GetComponent<Hex>()); }
            }
            SpawnNode.GetComponent<Hex>().EntityToSpawn = character.GetComponent<Entity>();
            index++;
        }
        return playerHexes;
    }

    RoomSide OpositeDirection(RoomSide direction)
    {
        switch (direction)
        {
            case RoomSide.Down:
                return RoomSide.Top;
            case RoomSide.Top:
                return RoomSide.Down;
            case RoomSide.Left:
                return RoomSide.Right;
            case RoomSide.Right:
                return RoomSide.Left;
        }
        return RoomSide.Left;
    }

    List<Hex> BuildStartRoom(int q, int r, RoomSide directionBuilding)
    {
        List<GameObject> roomsAvailable = RoomsWithWallsNotInDirection(OpositeDirection(directionBuilding));
        int RoomDatabaseIndex = Random.Range(0, roomsAvailable.Count);
        Room RoomPrefab = roomsAvailable[RoomDatabaseIndex].GetComponent<Room>();
        int width = RoomPrefab.width;
        int height = RoomPrefab.height;
        Node StartNode = hexMap.GetNode(q, r);
        string RoomName = ((char)((int)('A') + RoomIndex)).ToString();
        List<Hex> hexes = hexRoomBuilder.BuildRoomBySize(StartNode, height, width, RoomName, directionBuilding, 0, 0);

        if (hexes != null)
        {
            GameObject Room = Instantiate(roomsAvailable[RoomDatabaseIndex]);
            Room.GetComponent<Room>().start = true;
            BuildRoomShell(hexes, StartNode, directionBuilding, width, height, Room, RoomName, 0, 0, Vector3.zero);
        }
        return hexes;
    }

    void BuildRoomShell(List<Hex> hexes, Node StartNode, RoomSide directionBuilding, int width, int height, GameObject Room, string RoomName, int heightOffset, int widthOffset, Vector3 DoorOffset)
    {
        switch (directionBuilding)
        {
            case RoomSide.Top:
                Room.transform.position = (StartNode.transform.position + (Vector3.back * .735f * height) + (Vector3.right * widthOffset * 1.605f) + Vector3.down *.1f) - DoorOffset;
                break;
            case RoomSide.Right:
                Room.transform.position = (StartNode.transform.position + (Vector3.left * .78f * width) + (Vector3.back * heightOffset * 1.47f) + Vector3.down * .1f) - DoorOffset;
                break;
            case RoomSide.Left:
                Room.transform.position = (StartNode.transform.position + (Vector3.right * .73f * width) + (Vector3.back * heightOffset * 1.47f) + Vector3.down * .1f) - DoorOffset;
                break;
            case RoomSide.Down:
                Room.transform.position = (StartNode.transform.position + (Vector3.forward * .735f * height) + (Vector3.right * widthOffset * 1.605f) + Vector3.down * .1f) - DoorOffset;
                break;
        }
        DoorNew[] doors = Room.GetComponentsInChildren<DoorNew>();
        List<GameObject> ObjectToBeDestroyed = new List<GameObject>();
        foreach(DoorNew door in doors)
        {
            if (door.CheckIfOnAnotherWall()) { ObjectToBeDestroyed.Add(door.gameObject); }
        }
        foreach(GameObject obj in ObjectToBeDestroyed)
        {
            DestroyImmediate(obj);
        }
        DoorNew[] newDoors = Room.GetComponentsInChildren<DoorNew>();
        foreach (DoorNew doorNew in newDoors)
        {
            //doorNew.CheckIfOnAnotherWall();
            Door door = doorNew.CreateDoorHex(RoomName);
            door.RoomComingFrom = Room;
            door.HexesInRoom = hexes;
            doorsAvailable.Add(door);
        }
    }

    int attempts = 0;
    List<GameObject> charactersAvailableForChest;
    void CreateNextRoom()
    {
        attempts++;
        if (CurrentChallengeRating <= 0) { return; }
        if (RoomsMade.Count >= MaxRoomNumber) { return; }
        if (doorsAvailable.Count <= 0) { return; }
        if (attempts > 10) { return; }
        Door door = doorsAvailable[Random.Range(0, doorsAvailable.Count)];
        Node node = door.GetComponent<Node>();
        List<Hex> NewHexes = BuildRoom(node.q, node.r, door.RoomSideToBuild, door);
        if (NewHexes == null)
        {
            CreateNextRoom();
            return;
        }
        door.hexesToOpenTo = NewHexes;
        PopulateRoom(NewHexes);
        doorsAvailable.Remove(door);
        CreateNextRoom();
    }

    void AddEnemies(List<Hex> hexes)
    {
        if (CurrentChallengeRating <= 0) { return; }
        List<Hex> NonEdgeHexes = GetNonEdgeHexes(hexes);
        int EnemiesToSpawn = Random.Range(2, 5);
        int RoomChallengeRating = 0;
        for (int i= 0; i< EnemiesToSpawn; i++)
        {
            if (CurrentChallengeRating - RoomChallengeRating <= 0) { break; }
            GameObject RandomEnemy = EnemyPool[Random.Range(0, EnemyPool.Count)];
            if (NonEdgeHexes.Count <= 0) { continue; }
            Hex RandomHex = NonEdgeHexes[Random.Range(0, NonEdgeHexes.Count)];
            RandomHex.EntityToSpawn = RandomEnemy.GetComponent<Entity>();
            NonEdgeHexes.Remove(RandomHex);
            RoomChallengeRating += RandomEnemy.GetComponent<EnemyCharacter>().EnemyChallengeRating;
            totalEnemiesOut++;
        }
        CurrentChallengeRating -= RoomChallengeRating;
    }

    void AddObstaclesToRoom(List<Hex> hexes)
    {
        List<Hex> NonEdgeHexes = GetNonEdgeHexes(hexes);
        int MaximumObstaclesShouldPlace = NonEdgeHexes.Count / 5;
        List<Hex> hexPool = new List<Hex>();
        foreach(Hex hex in hexes) { hexPool.Add(hex); }
        for (int i =0; i < MaximumObstaclesShouldPlace; i++)
        {
            if (hexPool.Count <= 0) { return; }
            int RandomLocation = Random.Range(0, hexPool.Count);
            Hex AttemptHex = hexPool[RandomLocation];
           if (HexNotNextToOtherObstacleOrDoor(AttemptHex))
            {
                int RandomObstacleIndex = Random.Range(0, ObstaclePool.Count);
                AttemptHex.EntityToSpawn = ObstaclePool[RandomObstacleIndex].GetComponent<Entity>(); ;
            }
            else
            {
                i--;
                hexPool.RemoveAt(RandomLocation);
            }
        }
    }

    List<GameObject> CharactersThatNeedAChest = new List<GameObject>();
    void AddChestsToRoom(List<Hex> hexes)
    {
        if (CurrentTreasureAmount <= 0) { return; }
        List<Hex> NonEdgeHexes = GetNonEdgeHexes(hexes);
        if (NonEdgeHexes.Count <= 0) { return; }
        Hex RandomHex = NonEdgeHexes[Random.Range(0, NonEdgeHexes.Count)];
        if (HexNotNextToOtherObstacleOrDoor(RandomHex))
        {
            CurrentTreasureAmount--;
            string chestCharacter = RepopulateAndSubtractCharacterFromList();
            if (Random.Range(0, 2) == 0) {
                RandomHex.chestFor = chestCharacter;
                RandomHex.EntityToSpawn = CombatChestPrefab.GetComponent<Entity>(); }
            else {
                RandomHex.chestFor = chestCharacter;
                RandomHex.EntityToSpawn = ExplorationChestPrefab.GetComponent<Entity>();
            }
        }
        else
        {
            Hex NewRandomHex = NonEdgeHexes[Random.Range(0, NonEdgeHexes.Count)];
            if (HexNotNextToOtherObstacleOrDoor(NewRandomHex))
            {
                CurrentTreasureAmount--;
                string chestCharacter = RepopulateAndSubtractCharacterFromList();
                if (Random.Range(0, 2) == 0) {
                    NewRandomHex.chestFor = chestCharacter;
                    NewRandomHex.EntityToSpawn = CombatChestPrefab.GetComponent<Entity>();
                }
                else {
                    NewRandomHex.chestFor = chestCharacter;
                    NewRandomHex.EntityToSpawn = ExplorationChestPrefab.GetComponent<Entity>();
                }
            }
        }
    }

    string RepopulateAndSubtractCharacterFromList()
    {
        if (CharactersThatNeedAChest.Count == 0)
        {
            foreach(GameObject character in PlayerCharacters)
            {
                CharactersThatNeedAChest.Add(character);
            }
        }

        int randomIndex = Random.Range(0, CharactersThatNeedAChest.Count);
        GameObject chestCharacter = CharactersThatNeedAChest[randomIndex];
        CharactersThatNeedAChest.RemoveAt(randomIndex);
        return chestCharacter.GetComponent<PlayerCharacter>().CharacterName;
    }

    bool HexNotNextToOtherObstacleOrDoor(Hex hex)
    {
        if (hex.GetComponent<Node>().edge) { return false; }
        if (hex.GetComponent<Door>() != null) { return false; }
        if (hex.EntityToSpawn != null) { return false; }
        List<Node> AdjacentNodes = hexMap.GetNeighborsNoRoom(hex.GetComponent<Node>());
        foreach(Node node in AdjacentNodes)
        {
            if (node.GetComponent<Hex>().EntityToSpawn != null) { return false; }
            if (node.GetComponent<Door>() != null) { return false; }
        }
        return true;
    }

    void SetNexHexes(List<Hex> hexes)
    {
        foreach(Hex hex in hexes)
        {
            hex.GetComponent<Node>().Used = true;
            //hex.GetComponent<HexWallAdjuster>().HideWall();
        }
    }

    List<Hex> GetNonEdgeHexes(List<Hex> hexes)
    {
        if (hexes == null) { return new List<Hex>(); }
        List<Hex> nonEdgeHexes = new List<Hex>();
        foreach (Hex hex in hexes)
        {
            if (!hex.GetComponent<Node>().edge && hex.GetComponent<Door>() == null) { nonEdgeHexes.Add(hex); }
        }
        return nonEdgeHexes;
    }

    List<Hex> GetEdgeHexes(List<Hex> hexes)
    {
        List<Hex> edgeHexes = new List<Hex>();
        foreach(Hex hex in hexes)
        {
            if (hex.GetComponent<Node>().edge) { edgeHexes.Add(hex); }
        }
        return edgeHexes;
    }

    List<Hex> BuildRoom(int q, int r, RoomSide directionBuilding, Door door)
    {
        List<GameObject> roomsAvailable;
        roomsAvailable = RoomsWithWallsNotInDirection(OpositeDirection(directionBuilding));
        int RoomDatabaseIndex = Random.Range(0, roomsAvailable.Count);
        Room RoomPrefab = roomsAvailable[RoomDatabaseIndex].GetComponent<Room>();
        int width = RoomPrefab.width;
        int height = RoomPrefab.height;
        if (directionBuilding == RoomSide.Top || directionBuilding == RoomSide.Down)
        {
            height = RoomPrefab.height + 1;
            width = RoomPrefab.width - 1;
        }
        Node StartNode = hexMap.GetNode(q, r);
        string OldRoomName = StartNode.RoomName[0];
        string RoomName = ((char)((int)('A') + RoomIndex)).ToString();
        int oldRoomHeight = door.door.GetComponentInParent<Room>().height;
        int oldRoomWidth = door.door.GetComponentInParent<Room>().width;

        int heightOffset = 0;
        int widthOffset = 0;
        Vector3 DoorOffset = Vector3.zero;
        if (directionBuilding == RoomSide.Left || directionBuilding == RoomSide.Right)
        {
            if (height > oldRoomHeight && oldRoomHeight != 0)
            {
                bool up = Random.Range(0, 2) == 1;
                if (up)
                {
                    heightOffset = ((height - oldRoomHeight))/2;
                }
                else
                {
                    heightOffset = -((height - oldRoomHeight))/2;
                }
            }
        }
        else if (directionBuilding == RoomSide.Down || directionBuilding == RoomSide.Top)
        {
            if (width > oldRoomWidth && oldRoomWidth != 0)
            {
                widthOffset = -((width - oldRoomWidth));
            }
            DoorOffset = Vector3.right * (StartNode.transform.position.x - door.door.GetComponentInParent<Room>().transform.position.x);
        }

        List<Hex> hexes = hexRoomBuilder.BuildRoomBySize(StartNode, height, width, RoomName, directionBuilding, heightOffset, widthOffset);
        if (hexes != null)
        {
            foreach (Hex hex in hexes) { hex.GetComponent<Node>().Used = true; }
            StartNode.AddRoomName(OldRoomName);
            attempts = 0;
            GameObject Room = Instantiate(roomsAvailable[RoomDatabaseIndex]);
            door.RoomOpeningTo = Room;
            door.GetComponent<Node>().AddRoomName(RoomName);
            BuildRoomShell(hexes, StartNode, directionBuilding, width, height, Room, RoomName, heightOffset, widthOffset, DoorOffset);
            RoomIndex++;
        }
        return hexes;
    }

    void ShowHexSet(List<Hex> hexes, string Room)
    {
        foreach (Hex hex in hexes)
        {
            hex.GetComponent<Node>().Used = true;
            hex.GetComponent<HexAdjuster>().AddRoomShown(Room);
            hex.ShowHexEditor();
            hex.ShowMoney();
            hex.GetComponent<Node>().isAvailable = true;
            if (hex.EntityToSpawn != null)
            {
                hex.GenerateCharacter();
            }
        }
    }

    void SetStartNode(Node node, string RoomName)
    {
        node.GetComponent<HexAdjuster>().RightRoomSide = RoomName;
        node.SetRoomName(RoomName);
        node.GetComponent<HexAdjuster>().SetHexToHalf();
        node.GetComponent<HexAdjuster>().RotateHexToTopMiddle();
        node.Shown = true;
    }
}
