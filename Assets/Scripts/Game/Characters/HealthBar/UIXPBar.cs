using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIXPBar : MonoBehaviour {

    public GameObject XP;
    public float Min = -155;
    public float Max = 0;
    float Total { get { return Max - Min; } }

    public Text CurrentXpText;
    public Text XpUntilNextLevelText;

    public Vector3 CurrentPos;
    bool LevelingUp = false;

    private void Start()
    {
        CurrentPos = XP.transform.localPosition;
    }

    public void SetXP(float xp, float xpUntilNextLevel)
    {
        CurrentXpText.text = xp.ToString();
        XpUntilNextLevelText.text = xpUntilNextLevel.ToString();

        float percentage = xp / xpUntilNextLevel;
        float XPos = percentage * Total + Min;
        XP.transform.localPosition = new Vector3(XPos, 0f, 0f);
        CurrentPos = new Vector3(XPos, 0f, 0f);
    }

    public void GainXp(float xp, float xpUntilNextLevel)
    {
        CurrentXpText.text = xp.ToString();
        XpUntilNextLevelText.text = xpUntilNextLevel.ToString();

        float percentage = xp / xpUntilNextLevel;
        float XPos = percentage * Total + Min;
        CurrentPos = new Vector3(XPos, 0f, 0f);
    }

    public void LevelUpAndGainXP(float xp, float xpUntilNextLevel)
    {
        CurrentXpText.text = xp.ToString();
        XpUntilNextLevelText.text = xpUntilNextLevel.ToString();

        float percentage = xp / xpUntilNextLevel;
        LevelingUp = true;
        float currentPosX = ((1 + percentage) * Total) + Min;
        CurrentPos = new Vector3(currentPosX, 0, 0);
    }

    private void Update()
    {
        if (LevelingUp)
        {
            if (XP.transform.localPosition.x < Max)
            {
                XP.transform.localPosition = Vector3.Lerp(XP.transform.localPosition, CurrentPos, .1f);
            }
            else
            {
                float currentPosX = (CurrentPos.x - Total);
                CurrentPos = new Vector3(currentPosX, XP.transform.localPosition.y, XP.transform.localPosition.z);
                XP.transform.localPosition = new Vector3(Min, XP.transform.localPosition.y, XP.transform.localPosition.z);
                LevelingUp = false;
            }
        }
        else
        if ((XP.transform.localPosition - CurrentPos).magnitude > 1f)
        {
            XP.transform.localPosition = Vector3.Lerp(XP.transform.localPosition, CurrentPos, .1f);
        }
    }
}
