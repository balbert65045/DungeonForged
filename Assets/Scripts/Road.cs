using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PathPart : MonoBehaviour
{
    public Path pathOn = null;
    public Color baseColor;

    public virtual void Remove()
    {
        
    }
    public void ShowPath()
    {
        pathOn.HighlightPath();
    }

    public void hidePath()
    {
        pathOn.ReturnToNormal();
    }

    public virtual void HighlightPath()
    {  }
    public virtual void ReturnToNormal()
    {    }  
}

public enum RoadDirection
{
    Up = 1,
    Right = 2,
    Down = 3,
    Left = 4
}
public class Road : PathPart
{
    public RoadDirection myDirection;

    public Image roadPiece1;
    public Image roadPiece2;
    public Image roadPiece3;

    public override void Remove()
    {
        Destroy(this.gameObject);
    }
    public override void HighlightPath()
    {
        Image[] roadParts = GetComponentsInChildren<Image>();
        foreach(Image roadPart in roadParts)
        {
            roadPart.color = Color.black;
        }
    }

    public override void ReturnToNormal()
    {
        Image[] roadParts = GetComponentsInChildren<Image>();
        foreach (Image roadPart in roadParts)
        {
            roadPart.color = baseColor;
        }
    }

    public void MoveOnPath()
    {
        StartCoroutine("MovingOnRoad");
    }

    IEnumerator MovingOnRoad()
    {
        yield return new WaitForSeconds(.5f);
        roadPiece1.color = Color.white;
        yield return new WaitForSeconds(.5f);
        roadPiece2.color = Color.white;
        yield return new WaitForSeconds(.5f);
        roadPiece3.color = Color.white;
    }
}
