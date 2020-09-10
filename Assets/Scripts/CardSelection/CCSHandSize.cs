using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CCSHandSize : MonoBehaviour {

	public void SetHandSize(int currentSize, int maxSize)
    {
        GetComponent<Text>().text = currentSize.ToString() + "/" + maxSize.ToString();
    }
}
