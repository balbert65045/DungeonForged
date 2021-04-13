using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardPanel : MonoBehaviour
{
    public UpgradeCardArea upgradeCardArea;
    // Start is called before the first frame update
    void Start()
    {
        string characterName = FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].CharacterName;
        GetComponentInChildren<CardSelectionPanel>().ShowCards(characterName, false);
    }

    public void ShowUpgradeCard(GameObject basicPrefabCard, GameObject upgradeCardPrefab)
    {
        upgradeCardArea.gameObject.SetActive(true);
        upgradeCardArea.ShowUpgradeCard(basicPrefabCard, upgradeCardPrefab);
    }

    public void HideUpgradeCard()
    {
        upgradeCardArea.DestroyInstances();
        upgradeCardArea.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
