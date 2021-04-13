using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ArtifactType
{
    HexAttackIncrease = 1,
    HexMoveIncrease = 2,
    HexDrawIncrease = 3,
    HexShieldIncrease = 4,
    ShieldStart = 5,
    DrawStart = 6,
    MoveStart = 7,
    AttackStart = 8,
}
public class Artifact : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public ArtifactType artifactType;
    public string Name;
    public string Description;
    Tooltip tip;

    private void Start()
    {
        tip = GetComponent<Tooltip>();
        tip.tooltipTitle = Name;
        tip.tooltipText = Description;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!GetComponentInParent<ArtifactPanel>())
        {
            FindObjectOfType<TooltipController>().ShowUITooltip(tip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FindObjectOfType<TooltipController>().HideUITooltip();
    }
}
