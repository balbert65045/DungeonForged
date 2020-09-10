using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxialHex  {
    public int q, r, s;

    public AxialHex(int q_, int r_) {
        q = q_;
        r = r_;
        s = -q_ - r_;
    }

}
