using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreSpot : MonoBehaviour
{
    public int price;
    public Text priceText;
    public string CharacterNameFor;


    public void BuyCard()
    {
        int goldHolding = FindObjectOfType<PlayerCurrency>().GoldHolding();
        if (goldHolding >= price)
        {
            FindObjectOfType<NewGroupStorage>().AddCardToStorage(CharacterNameFor, GetComponentInChildren<NewCard>().PrefabAssociatedWith);
            FindObjectOfType<PlayerCurrency>().SetGoldValue(goldHolding - price);
            this.gameObject.SetActive(false);
        }
    }

    public void SetPrice(int amount)
    {
        price = amount;
        priceText.text = amount.ToString();
    }
}
