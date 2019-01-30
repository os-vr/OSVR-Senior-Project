using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Check {
    GStatus CheckPoint(GTransform transform);

    GStatus CheckAll(List<GTransform> buffer);
}
