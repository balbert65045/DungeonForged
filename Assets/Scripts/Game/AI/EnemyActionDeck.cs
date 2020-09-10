using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionDeck : MonoBehaviour {

    public List<EnemyActionCard> discardedEnemyActions = new List<EnemyActionCard>();
    public List<EnemyActionCard> enemyActions = new List<EnemyActionCard>();
    public string CharacterNameLinkedTo;

    // Use this for initialization
    public void SetUpDeck () {
        enemyActions.AddRange(GetComponentsInChildren<EnemyActionCard>());
    }

    public EnemyActionCard GetRandomActionCard()
    {
        EnemyActionCard selectedCard = enemyActions[Random.Range(0, enemyActions.Count)];
        enemyActions.Remove(selectedCard);
        return selectedCard;
    }

    public void DiscardCard(EnemyActionCard discardedCard)
    {
        discardedEnemyActions.Add(discardedCard);
    }

    public void Shuffle()
    {
        enemyActions.AddRange(discardedEnemyActions);
        discardedEnemyActions.Clear();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
