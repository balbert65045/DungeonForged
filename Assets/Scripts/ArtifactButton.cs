using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactButton : MonoBehaviour
{
    public GameObject ChestObj;

    public void ShowArtifact()
    {
        ChestObj.GetComponent<Animator>().SetTrigger("Open");
        FindObjectOfType<ArtifactPanel>().ShowArtifact();
        this.gameObject.SetActive(false);
    }
}
