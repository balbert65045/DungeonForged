using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWallAdjuster : MonoBehaviour {

    public GameObject WallObjHalf;
    public GameObject WallObjCorner;
    public GameObject WallObjectSide;

    public List<FlatWall> myWalls = new List<FlatWall>();

    bool WallAlreadyApartOfRoom(string room)
    {
        foreach(FlatWall wall in myWalls)
        {
            if (wall == null) { continue; }
            if (wall.RoomLinkedTo.Contains(room))
            {
                return true;
            }
        }
        return false;
    }

    public void DestroyAllWalls()
    {
        foreach (FlatWall wall in myWalls)
        {
            if (wall != null)
            {
                DestroyImmediate(wall.gameObject);
            }
        }
    }

    public void CreateHexWall(int type, int side, string room)
    {
        GameObject myWall = null;
        if (WallAlreadyApartOfRoom(room)){ return; }
        switch (type)
        {
            //Side Hex
            case 0:
                myWall = Instantiate(WallObjectSide, this.transform);
                //left side
                if (side == 0)
                {
                    myWall.transform.localPosition = new Vector3(.892f, 0, -.13f);
                }
                //RightSide
                else if (side == 1)
                {
                    myWall.transform.localPosition = new Vector3(-.892f, 0, -.13f);
                }
                myWall.GetComponent<FlatWall>().LinkWallToRoom(room);
                break;
            //Half Hex
            case 1:
                myWall = Instantiate(WallObjHalf, this.transform);
                myWall.transform.localPosition = new Vector3(0, 0, -.13f);
                myWall.GetComponent<FlatWall>().LinkWallToRoom(room);
                break;
            //Corner Hex
            case 2:
                myWall = Instantiate(WallObjCorner);
                myWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                myWall.transform.SetParent(this.transform);
                if (side == 0)
                {
                    myWall.transform.localPosition = new Vector3(.5f, .3f, -.13f);
                }
                else
                {
                    myWall.transform.localPosition = new Vector3(-.5f, -.3f, -.13f);
                }
                myWall.GetComponent<FlatWall>().LinkWallToRoom(room);
                break;
        }
        myWalls.Add(myWall.GetComponent<FlatWall>());
        //myWall.gameObject.SetActive(false);
    }

    public void ShowWall()
    {
        List<string> roomsShown = GetComponent<HexAdjuster>().RoomsShown;
        foreach (string room in roomsShown)
        {
            foreach (FlatWall wall in myWalls)
            {
                if (wall == null) { continue; }
                if (wall.RoomLinkedTo.Contains(room))
                {
                    wall.gameObject.SetActive(true);
                }
            }
        }
    }

    public void HideWall()
    {
        List<string> roomsShown = GetComponent<HexAdjuster>().RoomsShown;
        foreach (FlatWall wall in myWalls)
        {
            if (wall == null) { continue; }
            bool disable = true;
            foreach (string room in roomsShown)
            {
                if (wall.RoomLinkedTo.Contains(room)) { disable = false; }
            }
            if (disable) { wall.gameObject.SetActive(false); }
        }
    }
}
