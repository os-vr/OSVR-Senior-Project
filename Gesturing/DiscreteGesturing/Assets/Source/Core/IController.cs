using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gestures
{
    public abstract class IController : MonoBehaviour {

        public abstract GTransform QueryGTransform();
        public abstract bool GestureActive();

    }
}
