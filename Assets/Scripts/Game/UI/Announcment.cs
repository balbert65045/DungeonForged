using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Announcment : MonoBehaviour {

    public Text announcmentText;
    public GameObject announcmentPanel;
    // Use this for initialization

    public void ShowText(string text)
    {
        StartCoroutine("showText", text);
    }

    IEnumerator showText(string text)
    {
        announcmentPanel.SetActive(true);
        announcmentText.text = text;
        yield return new WaitForSeconds(1f);
        announcmentPanel.SetActive(false);
    }

    public void hideText()
    {
        announcmentPanel.SetActive(false);
    }
}
