using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTopBar : MonoBehaviour
{
    private void OnLevelWasLoaded(int level)
    {
        if (FindObjectOfType<PlayerController>() != null)
        {
            GetComponent<Image>().enabled = false;
        }
        else
        {
            GetComponent<Image>().enabled = true;
        }
    }
    public void HideBar()
    {
        GetComponent<Image>().enabled = false;
    }
}
