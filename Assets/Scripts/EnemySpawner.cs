using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyToSpawn;

    void Start()
    {
        FindObjectOfType<TurnOrder>().AddCharacter(GetComponent<Entity>());
    }

    public void Spawn()
    {
        StartCoroutine("SpawnEnemy");
    }

    IEnumerator SpawnEnemy()
    {
        Hex HexOn = GetComponent<Obstacle>().HexOn;
        FindObjectOfType<MyCameraController>().SetTarget(HexOn.transform);
        yield return new WaitForSeconds(.3f);
        GetComponent<MeshRenderer>().enabled = false;
        HexOn.EntityToSpawn = EnemyToSpawn.GetComponent<Entity>();
        HexOn.GenerateCharacter();
        yield return new WaitForSeconds(.3f);
        FindObjectOfType<EnemyController>().CharacterEndedTurn();
        Destroy(this.gameObject);
    }
}
