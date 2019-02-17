using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gestures {

    [System.Serializable]
    public abstract class Check {
        protected GameObject visualizationObject;

        public abstract float CheckPasses(GTransform transform);
        public virtual void BuildEditor() { }

        public void VisualizeCheck(bool active) {
            visualizationObject.SetActive(active);
        }
    }

}