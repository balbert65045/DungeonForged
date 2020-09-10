using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Entity {

    public Vector3 Offset;
    public List<Vector2> OtherHexesOnTopOf = new List<Vector2>();
    public Vector3 Rotation;

}
