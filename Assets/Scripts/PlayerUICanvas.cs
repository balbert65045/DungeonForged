using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUICanvas : MonoBehaviour
{
    public GameObject canvasHolder;

    private void Awake()
    {
        PlayerUICanvas[] playerCanvas = FindObjectsOfType<PlayerUICanvas>();
        if (playerCanvas.Length > 1) { Destroy(playerCanvas[1].gameObject); }
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "Loading")
        {
            canvasHolder.SetActive(false);
        }
        else
        {
            canvasHolder.SetActive(true);
        }
    }
  
}
