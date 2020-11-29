using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour {

    public GameObject HPPrediction;
    public GameObject HpObject;
    public float Min = -33;
    public float Max = 32;
    float Total { get { return Max - Min; } }
    public Vector3 CurrentPos;
    bool predicting = false;

    public void SetHP(float percentage)
    {
        predicting = false;
        if (percentage < 0) { percentage = 0; }
        float currentPosX = (percentage * Total) + Min;
        CurrentPos = new Vector3(currentPosX, HpObject.transform.localPosition.y, HpObject.transform.localPosition.z);
    }

    public void ShowPredictedDamage(float percentage)
    {
        predicting = true;
        if (percentage < 0) { percentage = 0; }
        float currentPosX = (percentage * Total) + Min;
        HpObject.transform.localPosition = new Vector3(currentPosX, HpObject.transform.localPosition.y, HpObject.transform.localPosition.z);
    }

	
	// Update is called once per frame
	void Update () {
        if (predicting) { return; }
        if ((HpObject.transform.localPosition - CurrentPos).magnitude > 1f)
        {
            HpObject.transform.localPosition = Vector3.Lerp(HpObject.transform.localPosition, CurrentPos, .1f);
        }
    }
}
