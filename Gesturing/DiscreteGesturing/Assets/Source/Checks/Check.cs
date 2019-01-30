using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Check{

    GStatus CheckPasses(GTransform transform);
    void VisualizeCheck(bool active);
}
