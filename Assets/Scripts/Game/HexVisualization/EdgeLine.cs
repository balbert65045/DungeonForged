using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeLine : MonoBehaviour {

    public Character characterLinkedTo = null;
    public Material MovementMaterial;
    public Material AttackMaterial;
    public Material ArmorMaterial;
    public Material HealMaterial;

    LineRenderer line;

    void Start () {
        line = GetComponent<LineRenderer>();
    }

    public void SetCurrentMaterial(ActionType type)
    {
        line = GetComponent<LineRenderer>();
        switch (type)
        {
            case ActionType.Movement:
                GetComponent<LineRenderer>().material = MovementMaterial;
                break;
            case ActionType.Attack:
                GetComponent<LineRenderer>().material = AttackMaterial;
                break;
            case ActionType.Shield:
                GetComponent<LineRenderer>().material = ArmorMaterial;
                break;
            case ActionType.Heal:
                GetComponent<LineRenderer>().material = HealMaterial;
                break;
            default:
                GetComponent<LineRenderer>().material = HealMaterial;
                break;
        }
    }

    public void DestroyLine()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    public void CreateLine(Vector3[] points)
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = points.Length;
        line.SetPositions(points);
    }

}
