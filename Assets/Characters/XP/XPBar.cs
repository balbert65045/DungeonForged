using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPBar : MonoBehaviour {

    public GameObject XPObject;
    public float Min = -33;
    public float Max = 32;
    float Total { get { return Max - Min; } }

    public Vector3 CurrentPos;
    bool LevelingUp = false;

    public void SetXp(float percentage)
    {
        float currentPosX = (percentage * Total) + Min;
        CurrentPos = new Vector3(currentPosX, XPObject.transform.localPosition.y, XPObject.transform.localPosition.z); 
    }

    public void StartXP(float percentage)
    {
        float currentPosX = (percentage * Total) + Min;
        CurrentPos = new Vector3(currentPosX, XPObject.transform.localPosition.y, XPObject.transform.localPosition.z);
        XPObject.transform.localPosition = CurrentPos;
    }

    public void LevelUp(float percentage)
    {
        LevelingUp = true;
        float currentPosX = ((1 + percentage) * Total) + Min;
        CurrentPos = new Vector3(currentPosX, XPObject.transform.localPosition.y, XPObject.transform.localPosition.z);
    }

	
	void Update () {
        if (LevelingUp)
        {
            if (XPObject.transform.localPosition.x < Max)
            {
                XPObject.transform.localPosition = Vector3.Lerp(XPObject.transform.localPosition, CurrentPos, .1f);
            } else
            {
                float currentPosX = (CurrentPos.x - Total);
                CurrentPos = new Vector3(currentPosX, XPObject.transform.localPosition.y, XPObject.transform.localPosition.z);
                XPObject.transform.localPosition = new Vector3(Min, XPObject.transform.localPosition.y, XPObject.transform.localPosition.z);
                LevelingUp = false;
            }
        } else
        {
            if ((XPObject.transform.localPosition - CurrentPos).magnitude > 1f)
            {
                XPObject.transform.localPosition = Vector3.Lerp(XPObject.transform.localPosition, CurrentPos, .1f);
            }
        }
	}
}
