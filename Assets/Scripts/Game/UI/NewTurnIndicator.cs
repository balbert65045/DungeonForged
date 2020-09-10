using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTurnIndicator : MonoBehaviour {

    bool Shift = false;
    public void SetShift() { Shift = true; }

    void Update()
    {
        if (Shift)
        {
            if (transform.localPosition.magnitude > 1f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, .04f);
            }
            else
            {
                Shift = false;
            }
        }
    }
}
