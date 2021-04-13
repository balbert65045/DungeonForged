using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCanvas : MonoBehaviour
{
    public MapButton mapButton;
    public LocationMap locationMap;

    private void Awake()
    {
        MapCanvas[] mapCanvas = FindObjectsOfType<MapCanvas>();
        if (mapCanvas.Length > 1) { Destroy(mapCanvas[1].gameObject); }
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "Loading" || SceneManager.GetActiveScene().name == "Map")
        {
            HideMapButton();
        }
        else
        {
            ShowMapButton();
        }
    }

    public void HideMapButton()
    {
        mapButton.gameObject.SetActive(false);
    }

    public void ShowMapButton()
    {
        mapButton.gameObject.SetActive(true);
    }

    public void ShowMap()
    {
        locationMap.gameObject.SetActive(true);
    }

    public void HideMap()
    {
        locationMap.gameObject.SetActive(false);
    }
}
