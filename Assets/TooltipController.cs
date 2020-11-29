using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipController : MonoBehaviour
{

    public GameObject TooltipPanel;
    public Text Title;
    public Text Text;
    public LayerMask TooltipLayer;

    public Tooltip TooltipShowing = null;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;
        if (Physics.Raycast(ray, out Hit, 100f, TooltipLayer))
        {
            if (Hit.collider.GetComponent<Tooltip>() != null && Hit.collider.GetComponent<Tooltip>() != TooltipShowing)
            {
                ShowTooltip(Hit.collider.GetComponent<Tooltip>());
            }
            return;
        }
        HideTooltip();
    }

    void HideTooltip()
    {
        if (TooltipShowing != null)
        {
            TooltipPanel.SetActive(false);
            TooltipShowing = null;
        }
    }

    void ShowTooltip(Tooltip tip)
    {
        TooltipShowing = tip;
        TooltipPanel.SetActive(true);
        TooltipPanel.transform.position = Input.mousePosition - new Vector3(-100f, 75f);
        Title.text = tip.tooltipTitle;
        Text.text = tip.tooltipText;
    }
}
