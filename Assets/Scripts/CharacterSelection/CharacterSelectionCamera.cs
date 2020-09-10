using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionCamera : MonoBehaviour {

    bool focusingOnTarget = false;
    public Vector3 target;
    public Vector3 StartPosition;
    public Vector3 DifferencePos = new Vector3(2, -1, -1);

    // Use this for initialization
    void Start () {
        StartPosition = transform.position;
    }
	
    public void setTarget(Vector3 pos)
    {
        target = pos - DifferencePos;
        focusingOnTarget = true;
    }

    public void ClearTarget()
    {
        focusingOnTarget = false;
    }

	void Update () {
		if (focusingOnTarget)
        {
            if ((target - transform.position).magnitude > .2f)
            {
                transform.position = Vector3.Lerp(transform.position, target, .02f);
            }
        }
        else
        {
            if ((StartPosition - transform.position).magnitude > .2f)
            {
                transform.position = Vector3.Lerp(transform.position, StartPosition, .02f);
            }
        }
	}
}
