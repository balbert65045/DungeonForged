using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Location
{
    Enemy = 1,
    Shop = 2,
    Rest = 3,
    Chest = 4,
    Start = 5,
    Furnace = 6,
    Boss = 7,
    Anvil = 8,
    Artifact = 9
}
public class LocationArea : PathPart, IPointerEnterHandler, IPointerDownHandler
{
    public bool Visible = false;
    public int X;
    public int Y;

    public Location myLocation;

    public Sprite Enemy;
    public Sprite Boss;
    public Sprite Shop;
    public Sprite Rest;
    public Sprite Chest;
    public Sprite StartSprite;
    public Sprite Furnace;
    public Sprite Anvil;
    public Sprite Artifact;

    public Color UnusedColor;

    public Image LocationSprite;
    // Start is called before the first frame update

    bool Showing = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (FindObjectOfType<PlayerMapController>() == null || FindObjectOfType<PlayerMapController>().Moving) { return; }
        if (pathOn != null)
        {
            Showing = true;
            ShowPath();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (FindObjectOfType<PlayerMapController>() == null || FindObjectOfType<PlayerMapController>().Moving) { return; }
        if (pathOn != null)
        {
            Showing = false;
            hidePath();
            FindObjectOfType<PlayerMapController>().MoveOnPath(pathOn);
        }
    }

    bool OverThisPath(List<RaycastResult> results)
    {
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<LocationArea>() != null && result.gameObject.GetComponent<LocationArea>() == this)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        if (Showing )
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            if (!OverThisPath(raysastResults))
            {
                Showing = false;
                hidePath();
            }
        }
    }

    public override void Remove()
    {
        Visible = false;
        pathOn = null;
        LocationSprite.color = new Color(0, 0, 0, 0);
    }

    public override void HighlightPath()
    {
        LocationSprite.color = Color.black;
    }

    public override void ReturnToNormal()
    {
        LocationSprite.color = UnusedColor;
    }

    void Start()
    {
        LocationSprite = GetComponent<Image>();
    }

    public void SetAsArtifactArea()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Artifact;
        myLocation = Location.Artifact;
    }

    public void SetAsRestArea()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Rest;
        myLocation = Location.Rest;
    }

    public void SetAsEnemy()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Enemy;
        myLocation = Location.Enemy;
    }

    public void SetAsBoss()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Boss;
        myLocation = Location.Boss;
    }

    public void SetAsShop()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Shop;
        myLocation = Location.Shop;
    }

    public void SetAsChest()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Chest;
        myLocation = Location.Chest;
    }

    public void SetAsAnvil()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Anvil;
        myLocation = Location.Anvil;
    }

    public void SetAsFurnace()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = Furnace;
        myLocation = Location.Furnace;
    }

    public void SetAsStart()
    {
        LocationSprite = GetComponent<Image>();
        LocationSprite.sprite = StartSprite;
        myLocation = Location.Start;
    }

    public void ChangeLevelToLocation()
    {
        switch (myLocation)
        {
            case Location.Enemy:
                FindObjectOfType<NewGroupStorage>().IncrimentChallengeRating();
                FindObjectOfType<LevelManager>().LoadLevelWithLoading();
                break;
            case Location.Boss:
                FindObjectOfType<NewGroupStorage>().SetNextLevelAsBoss();
                FindObjectOfType<LevelManager>().LoadLevelWithLoading();
                break;
            case Location.Shop:
                FindObjectOfType<LevelManager>().LoadLevel("Store");
                break;
            case Location.Rest:
                FindObjectOfType<LevelManager>().LoadLevel("Rest");
                break;
            case Location.Chest:
                FindObjectOfType<LevelManager>().LoadLevel("ChestRoom");
                break;
            case Location.Furnace:
                FindObjectOfType<LevelManager>().LoadLevel("Furnace");
                break;
            case Location.Anvil:
                FindObjectOfType<LevelManager>().LoadLevel("Anvil");
                break;
            case Location.Artifact:
                FindObjectOfType<LevelManager>().LoadLevel("Artifact");
                break;
        }
    }


    public void SetAsUnused()
    {
        LocationSprite.color = UnusedColor;
        Visible = true;
    }

    public void SetAsUsed()
    {
        LocationSprite.color = Color.white;
        Visible = true;
    }
}
