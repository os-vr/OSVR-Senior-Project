using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IController : MonoBehaviour {

    public abstract GTransform QueryGTransform();
    public abstract bool GestureActive();
    public abstract void UpdateController();

}
