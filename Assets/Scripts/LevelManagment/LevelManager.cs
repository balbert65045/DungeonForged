using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    // Use this for initialization

    public float autoLoadNextLevelAfter;

    private void Awake()
    {
        LevelManager[] levelManagers = FindObjectsOfType<LevelManager>();
        if (levelManagers.Length > 1) { Destroy(levelManagers[1].gameObject); }
        DontDestroyOnLoad(this.gameObject);
        
    }

    void Start()
    {
        if (autoLoadNextLevelAfter <= 0)
        {
            Debug.Log("Auto Load Disabled, use a positive number in seconds");
        } else
        {
            Invoke("LoadNextLevel", autoLoadNextLevelAfter);
        }
    }

    private void Update()
    {
        
    }

    public void LoadLevelWithLoading()
    {
        SceneManager.LoadScene("Loading");
    }

    public void LoadNextLevelFromIndex()
    {
        if (FindObjectOfType<NewGroupStorage>().BossNext) { SceneManager.LoadSceneAsync("Boss"); }
        else
        {
            //int randIndex = Random.Range(0, 3);
            //string Letter = "A";
            //if (randIndex == 1) { Letter = "B"; }
            //else if (randIndex == 2) { Letter = "C"; }
            int level = FindObjectOfType<NewGroupStorage>().LevelIndex;
            SceneManager.LoadSceneAsync("New Level " + level.ToString());
        }
    }

    public void LoadLevelWithDelay(string levelName, float timeDelay)
    {
        IEnumerator LoadTheLevel = LoadLevel(levelName, timeDelay);

        StartCoroutine(LoadTheLevel);
    }

    IEnumerator LoadLevel(string levelname, float delaytime)
    {
        yield return new WaitForSeconds(delaytime);
        SceneManager.LoadScene(levelname);
    }

	public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitRequest()
    {
       Application.Quit();
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
