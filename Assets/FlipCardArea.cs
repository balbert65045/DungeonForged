using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCardArea : MonoBehaviour
{
    public GameObject Area;
    public void ShowArea()
    {
        Area.SetActive(true);
    }

    public void HideArea()
    {
        Area.SetActive(false);
    }
}
