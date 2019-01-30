using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GestureRenderer : MonoBehaviour {
    public abstract void SetPositions(GTransformBuffer buffer);
}
