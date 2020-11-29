using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLoot : MonoBehaviour {

    public GameObject[] Positions;
    public GameObject Panel;
    PlayerCharacterType PCT;

    public void AddCardToStorage(NewCard card)
    {
        string CharacterNameForLoot = "";
        switch (PCT)
        {
            case PlayerCharacterType.Knight:
                CharacterNameForLoot = "Knight";
                break;
            case PlayerCharacterType.Crossbow:
                CharacterNameForLoot = "Huntress";
                break;
        }
        FindObjectOfType<NewGroupStorage>().AddCardToStorage(CharacterNameForLoot, card.PrefabAssociatedWith);
        HidePanel();
        if (GetComponentInParent<LevelClearedPanel>() != null)
        {
            GetComponentInParent<LevelClearedPanel>().ShowNextLoot();
        }
    }

    void HidePanel()
    {
        Panel.SetActive(false);
    }

	public void ShowThreeCards(PlayerCharacterType characterType)
    {
        PCT = characterType;
        Panel.SetActive(true);
        ClearOldCards();
        GameObject[] cardloot = FindObjectOfType<CardDatabase>().Select3RandomCards(characterType);
        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(cardloot[i], Positions[i].transform);
            card.GetComponent<NewCard>().PrefabAssociatedWith = cardloot[i];
            card.transform.localPosition = Vector3.zero;
            card.transform.rotation = Quaternion.identity;
        }
    }

    void ClearOldCards()
    {
        foreach(GameObject position in Positions)
        {
            if (position.GetComponentInChildren<NewCard>() != null)
            {
                Destroy(position.GetComponentInChildren<NewCard>().gameObject);
            }
        }
    }
}
