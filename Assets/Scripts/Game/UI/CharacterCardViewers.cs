using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCardViewers : MonoBehaviour {

    public CharacterCardViewer[] MyViewers;
    int index = 0;

    public GameObject Arrow;
    public bool Hidden = true;
    Vector3 NewPosition;

    private void Start()
    {
        NewPosition = transform.localPosition;
    }

    private void Update()
    {
        if ((NewPosition - transform.localPosition).magnitude > .1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, NewPosition, .2f);
        }
    }

    public void ToggleHidden()
    {
        if (Hidden)
        {
            ShowCardViewer();
        }
        else
        {
            HideCardViewer();
        }
    }

    public void HideCardViewer()
    {
        Arrow.transform.localEulerAngles = new Vector3(0, 180, 0);
        Hidden = true;
        NewPosition = new Vector3(transform.localPosition.x + 134.4f, transform.localPosition.y);
    }

    public void ShowCardViewer()
    {
        Arrow.transform.localEulerAngles = new Vector3(0, 0, 0);
        Hidden = false;
        NewPosition = new Vector3(transform.localPosition.x - 134.4f, transform.localPosition.y);
    }

    public void AddCharacter(PlayerCharacter character)
    {
        MyViewers[index].SetUpCharacter(character);
        MyViewers[index].gameObject.SetActive(true);
        index++;
    }
}
