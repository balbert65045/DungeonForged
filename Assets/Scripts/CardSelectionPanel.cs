using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionPanel : CCSCardAreaPanel
{
    NewCard CurrentSelectedCard;
    public GameObject SelectionObj;
    public GameObject DestroyButton;
    public int MaxCardsToDestroy = 1;
    int CurrentCardsDestroyed = 0;

   public void CardSelected(NewCard card)
    {
        if (GetComponentInParent<DestroyCardPanel>() != null)
        {
            if (CurrentCardsDestroyed >= MaxCardsToDestroy) { return; }
            if (CurrentSelectedCard != null)
            {
                UnSelectCurrentCard();
            }
            if (CurrentSelectedCard == card)
            {
                DestroyButton.SetActive(false);
                CurrentSelectedCard = null;
                return;
            }
            CurrentSelectedCard = card;
            SelectionObj.transform.SetParent(card.transform.parent);
            SelectionObj.transform.localPosition = Vector3.zero;
            SelectionObj.SetActive(true);
            SelectionObj.transform.SetAsFirstSibling();
            DestroyButton.SetActive(true);
        }
        else if (GetComponentInParent<UpgradeCardPanel>() != null)
        {
            CurrentSelectedCard = card;
            GetComponentInParent<UpgradeCardPanel>().ShowUpgradeCard(card.PrefabAssociatedWith, card.UpgradePrefab);
        }
    }

    public void UnSelectCurrentCard()
    {
        SelectionObj.SetActive(false);
    }

    public void DestroyCard()
    {
        FindObjectOfType<NewGroupStorage>().RemoveCardToStorage(FindObjectOfType<NewGroupStorage>().MyGroupCardStorage[0].CharacterName, CurrentSelectedCard.PrefabAssociatedWith);
        Destroy(CurrentSelectedCard.gameObject);
        DestroyButton.SetActive(false);
        UnSelectCurrentCard();
        CurrentSelectedCard = null;
        CurrentCardsDestroyed++;
    }

    public void UpgradeCardPanel()
    {
        //GetComponentInParent<UpgradeCardPanel>().ShowUpgrade(CurrentSelectedCard.PrefabAssociatedWith);
    }
}
