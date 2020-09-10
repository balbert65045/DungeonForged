using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour {

    public NewHand hand;
    public DrawPile DrawPile;
    public DiscardPile DiscardPile;
    public BurnPile BurnPile;
    public EnergyAmount EnergyAmount;

    public GameObject ExaustCardPrefab;

    public void AddExaustToDiscardPile()
    {
        GameObject exaustCard = Instantiate(ExaustCardPrefab, this.transform);
        StartCoroutine("ShowCardAndPutInDiscard", exaustCard);
    }

    IEnumerator ShowCardAndPutInDiscard(GameObject ExaustCard)
    {
        ExaustCard.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(2f);
        DiscardPile.DiscardCard(ExaustCard.GetComponent<NewCard>());
    }
}
