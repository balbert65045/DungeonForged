using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

    public GameObject OutLine;
    public GameObject DoorPrefab;
    public GameObject ExitPrefab;

    public int goldHolding = 0;
    public GameObject GoldPrefabSmall;
    public GameObject GoldPrefabMedium;
    public GameObject GoldPrefabLarge;
    public GameObject GoldHolding;

    public string chestFor;
    public Entity EntityToSpawn;
    public float EntityOffset = 0.1f;
    public Entity EntityHolding;

    public bool MovedTo = false;

    public Material InvisibleMaterial;
    public Material SlightlyVisibleMaterial;
    public Material OGMaterial;

    public Material previousMaterial { get; set; }
    public Node HexNode { get; set; }

    private void Awake()
    {
        previousMaterial = OGMaterial;
        HexNode = GetComponent<Node>();
        if (!HexNode.Shown)
        {
            HideHex();
        }
    }

    public bool HasEnemy() { return EntityHolding != null && EntityHolding.GetComponent<EnemyCharacter>() != null; }
    public EnemyCharacter GetEnemy() { return EntityHolding.GetComponent<EnemyCharacter>(); }
    public bool HasPlayer() { return EntityHolding != null && EntityHolding.GetComponent<PlayerCharacter>() != null; }
    public bool HasCharacter() { return EntityHolding != null && EntityHolding.GetComponent<Character>() != null; }

    public void OpenHex(string roomName)
    {
        GetComponent<Node>().isAvailable = true;
        GetComponent<HexAdjuster>().AddRoomShown(roomName);
        if (GetComponent<Node>().edge) {GetComponent<HexAdjuster>().RevealRoomEdge(); }
        //GetComponent<HexWallAdjuster>().ShowWall();
        if (!GetComponent<Node>().edge) { GetComponent<Hex>().ShowHex(); }
        HexNode.Shown = true;
        ShowMoney();
        if (EntityToSpawn != null) { CreateCharacter(); }
    }

    public int PickUpMoney()
    {
        Destroy(GoldHolding);
        int gold = goldHolding;
        goldHolding = 0;
        return gold;
    }

    public void ShowMoney()
    {
        if (GoldHolding != null) { Destroy(GoldHolding); }
        if (goldHolding > 0 && goldHolding < 6)
        {
            GoldHolding = Instantiate(GoldPrefabSmall, this.transform);
            GoldHolding.transform.localPosition = new Vector3(0, 0, -.15f);
            GoldHolding.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
        else if (goldHolding > 5 && goldHolding < 11)
        {
            GoldHolding = Instantiate(GoldPrefabMedium, this.transform);
            GoldHolding.transform.localPosition = new Vector3(0, -.1f, -.15f);
            GoldHolding.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
        else if (goldHolding > 10)
        {
            GoldHolding = Instantiate(GoldPrefabLarge, this.transform);
            GoldHolding.transform.localPosition = new Vector3(0, 0f, -.15f);
            GoldHolding.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        }
    }
    
    public void CharacterMovingToHex() { MovedTo = true; }
    public void CharacterArrivedAtHex() { MovedTo = false; }

    public void ShowHex()
    {
        if (!HexNode.Shown && !HexNode.edge)
        {
            //GetComponent<MeshRenderer>().material = OGMaterial;
            OutLine.SetActive(true);
        }
    }

    public void ShowHexEditor()
    {
        if (!GetComponent<Node>().edge)
        {
            OutLine.SetActive(true);
        }
        //GetComponent<MeshRenderer>().sharedMaterial = OGMaterial;
    }

    public void HideHexEditor()
    {
    //    GetComponent<MeshRenderer>().sharedMaterial = InvisibleMaterial;
        OutLine.SetActive(false);
    }

    public void HideHex()
    {
     //   GetComponent<MeshRenderer>().material = InvisibleMaterial;
        OutLine.SetActive(false);
    }

    public void slightlyShowHex()
    {
        GetComponent<MeshRenderer>().material = SlightlyVisibleMaterial;
    }

    public bool HasSameType(Character.CharacterType CT)
    {
        if (EntityHolding != null)
        {
            if (EntityHolding.GetComponent<Character>() != null)
            {
                return EntityHolding.GetComponent<Character>().myCT == CT;
            }
            else
            {
                return false;
            }
        }

        return true;
    }


    public GameObject CreateCharacter()
    {
        GameObject objectMade = GenerateCharacter();
        //if (EntityHolding.GetComponent<EnemyCharacter>() != null)
        //{
        //    FindObjectOfType<EnemyController>().LinkSpawnedCharacter(EntityHolding.GetComponent<EnemyCharacter>());
        //}
        return objectMade;
    }

    public GameObject GenerateCharacter()
    {
        if (EntityToSpawn != null)
        {
            Vector3 StartPos = new Vector3(transform.position.x, transform.position.y + EntityOffset, transform.position.z);
            Vector3 startingRot = new Vector3(0, -90, 0);
            if (EntityToSpawn.GetComponent<CardChest>() != null)
            {
                Transform interactionParent = FindObjectOfType<InteractionObjects>().gameObject.transform;
                EntityHolding = Instantiate(EntityToSpawn.gameObject, StartPos, Quaternion.Euler(startingRot), interactionParent).GetComponent<Entity>();
                OGMaterial = EntityHolding.GetComponent<CardChest>().HexOnMaterial;
                GetComponent<MeshRenderer>().material = OGMaterial;
                EntityHolding.GetComponent<CardChest>().SetCharacter(chestFor);
            }
            else if (EntityToSpawn.GetComponent<Obstacle>() != null)
            {
                StartPos = transform.position;
                Vector3 offset = EntityToSpawn.GetComponent<Obstacle>().Offset;
                EntityHolding = Instantiate(EntityToSpawn.gameObject, this.transform).GetComponent<Entity>();
                EntityHolding.transform.localPosition = offset;
                EntityHolding.transform.rotation = Quaternion.Euler(EntityToSpawn.GetComponent<Obstacle>().Rotation);

                HexMapController hexMap = FindObjectOfType<HexMapController>();
                hexMap.CreateTable();
                foreach (Vector2 delta in EntityHolding.GetComponent<Obstacle>().OtherHexesOnTopOf)
                {
                    LinkAdditionalHexesToEntity(delta);
                }
            }
            else
            {
                CharacterHolder CH = FindObjectOfType<CharacterHolder>();
                if (EntityToSpawn.GetComponent<EnemyCharacter>() != null)
                {
                    startingRot = new Vector3(0, 90, 0);
                }
                EntityHolding = Instantiate(EntityToSpawn.gameObject, StartPos, Quaternion.Euler(startingRot), CH.transform).GetComponent<Entity>();
            }
            EntityHolding.StartOnHex(this);
        }
        return EntityHolding.gameObject;
    }

    void LinkAdditionalHexesToEntity(Vector2 delta)
    {
        HexMapController hexMap = FindObjectOfType<HexMapController>();
        Node myNode = GetComponent<Node>();
        Node node = hexMap.GetNode(myNode.q + (int)delta.x, myNode.r + (int)delta.y);
        node.GetComponent<Hex>().EntityHolding = EntityHolding;
    }

    public void ShowHexEnd()
    {
        if (GetComponent<EndHex>() != null)
        {
            GetComponent<EndHex>().ShowObjectsHiding();
        }
    }

    public void HideHexEnd()
    {
        if (GetComponent<EndHex>() != null)
        {
            GetComponent<EndHex>().HideObjectsHiding();
        }
    }

    public void AddEntityToHex(Entity entity)
    {
        EntityHolding = entity;
    }

    public void RemoveEntityFromHex()
    {
        MovedTo = false;
        EntityHolding = null;
    }

    public void returnToPreviousColor()
    {
        //GetComponent<MeshRenderer>().material = OGMaterial;
    }

    public void UnHighlight()
    {
        GetComponent<MeshRenderer>().material = OGMaterial;
        previousMaterial = GetComponent<MeshRenderer>().material;
    }

}
