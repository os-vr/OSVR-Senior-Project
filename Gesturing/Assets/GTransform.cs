using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTransform {
    Vector3 position;
    Quaternion rotation;

    GTransform(Vector3 pos, Quaternion quat)
    {
        position = pos;
        rotation = quat;
    }
}
