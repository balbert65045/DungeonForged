using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardArea : MonoBehaviour
{

    public GameObject NormalCardSpot;
    public GameObject UpgradeCardSpot;

    public void ShowUpgradeCard(GameObject BasicCardPrefab, GameObject UpgradeCardPrefab)
    {
        GameObject NormalCard = Instantiate(BasicCardPrefab, NormalCardSpot.transform);
        CenterCard(NormalCard, BasicCardPrefab);
        GameObject UpgradeCard = Instantiate(UpgradeCardPrefab, UpgradeCardSpot.transform);
        CenterCard(UpgradeCard, UpgradeCardPrefab);
    }

    public void DestroyInstances()
    {
        Destroy(NormalCardSpot.transform.GetChild(0).gameObject);
        Destroy(UpgradeCardSpot.transform.GetChild(0).gameObject);
    }

    public void UpgradeCard()
    {
        NewGroupStorage storage = FindObjectOfType<NewGroupStorage>();
        string characterName = storage.MyGroupCardStorage[0].CharacterName;
        storage.RemoveCardToStorage(characterName, NormalCardSpot.GetComponentInChildren<NewCard>().PrefabAssociatedWith);
        storage.AddCardToStorage(characterName, UpgradeCardSpot.GetComponentInChildren<NewCard>().PrefabAssociatedWith);
        FindObjectOfType<LevelManager>().LoadLevel("Map");
    }

    void CenterCard(GameObject card, GameObject prefab)
    {
        card.transform.localPosition = Vector3.zero;
        card.transform.localScale = new Vector3(1, 1, 1);
        card.GetComponent<NewCard>().PrefabAssociatedWith = prefab;
    }
}
