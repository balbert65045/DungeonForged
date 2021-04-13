using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestButton : MonoBehaviour
{
    public PlayerCharacterType myCT;
    public GameObject ChestObj;

    public void ShowLoot()
    {
        ChestObj.GetComponent<Animator>().SetTrigger("Open");
        FindObjectOfType<CardLoot>().ShowThreeCards(myCT);
        this.gameObject.SetActive(false);
    }
}
