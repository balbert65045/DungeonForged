using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEVisualization : MonoBehaviour
{
    public Material AOEMaterial;
    public GameObject AOESprite;

    GameObject AOEIndication = null;
    int Uses = 0;

    public void CreateAOESprite()
    {
        if (AOEIndication == null)
        {
            AOEIndication = Instantiate(AOESprite, this.transform);
        }
        Uses++;
    }

    public bool DestroyAOESprite()
    {
        if (AOEIndication != null)
        {
            Uses--;
            if (Uses == 0)
            {
                Destroy(AOEIndication);
                return true;
            }
        }
        return false;
    }
}
