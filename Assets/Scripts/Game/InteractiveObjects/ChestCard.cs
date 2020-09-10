using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChestCard : MonoBehaviour, IPointerClickHandler
{

    public GameObject SelectionPanelPrefab;
    public GameObject SelectionPanel;
    public ChestPanel chestPanel;

    public void setUpCard(ChestPanel panel, GameObject selectionPanelPrefab)
    {
        chestPanel = panel;
        SelectionPanelPrefab = selectionPanelPrefab;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
    }

    public void CardSelected()
    {
        SelectionPanel = Instantiate(SelectionPanelPrefab, this.transform.parent);
        SelectionPanel.transform.SetAsFirstSibling();
    }

    public void CardDeSelected()
    {
        if (SelectionPanel != null)
        {
            Destroy(SelectionPanel);
        }
    }
}
