using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMap : MonoBehaviour {

    public GameObject Character1;
    public GameObject Character2;

    public GameObject Level1Spot;
    public GameObject Level2Spot;
    public GameObject Level3Spot;
    public GameObject LevelBSpot;

    Vector3 SpotMovingTo;
    bool Moving = false;

    public void SetCharacterPosition(int levelStart)
    {
        if (levelStart == 1)
        {
            Character1.transform.localPosition = new Vector3(-461.9f, Character1.transform.localPosition.y);
            Character2.transform.localPosition = new Vector3(-461.9f, Character2.transform.localPosition.y);
        }
        else if (levelStart == 2)
        {
            Character1.transform.localPosition = new Vector3(-159.1f, Character1.transform.localPosition.y);
            Character2.transform.localPosition = new Vector3(-159.1f, Character2.transform.localPosition.y);
        }
        else if (levelStart == 3)
        {
            Character1.transform.localPosition = new Vector3(165.3f, Character1.transform.localPosition.y);
            Character2.transform.localPosition = new Vector3(165.3f, Character2.transform.localPosition.y);
        }
    }

    public void MoveToLevel()
    {
        int levelNumber = FindObjectOfType<NewGroupStorage>().LevelIndex;
        levelNumber++;
        if (levelNumber == 2)
        {
            Moving = true;
            SpotMovingTo = Level2Spot.transform.localPosition;
        }
        else if (levelNumber == 3)
        {
            Moving = true;
            SpotMovingTo = Level3Spot.transform.localPosition;
        }
        else if (levelNumber == 4)
        {
            Moving = true;
            SpotMovingTo = LevelBSpot.transform.localPosition;
        }
        else
        {
            return;
        }
    }

    void Update()
    {
        if (Moving)
        {
            if (Mathf.Abs(Character1.transform.localPosition.x - SpotMovingTo.x) > 10f)
            {
                Character1.transform.localPosition = Vector3.Lerp(Character1.transform.localPosition, new Vector3(SpotMovingTo.x, Character1.transform.localPosition.y), .02f);
                Character2.transform.localPosition = Vector3.Lerp(Character2.transform.localPosition, new Vector3(SpotMovingTo.x, Character2.transform.localPosition.y), .02f);
            }
            else
            {
                FindObjectOfType<NewGroupStorage>().IncrimentChallengeRating();
                FindObjectOfType<LevelManager>().LoadLevelWithLoading();
            }
        }
    }
}
