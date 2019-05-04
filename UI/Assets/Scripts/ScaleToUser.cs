using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace OSVR.UI{
/// <summary>
/// Makes items, typically text, change size based on distance from user. 
/// Items still foreshorten, but not at a rate that makes them hard to see.
/// </summary>
/// <remarks>
/// Changes the physical size, in relation to a given object relative to the user.
/// 
/// </remarks>
    public class ScaleToUser : MonoBehaviour
    {
        /// <summary>
        /// The camera that the user uses to see the world
        /// </summary>
        [Tooltip("The camera that the user uses to see the world")]
        public GameObject userCamera;
        /// <summary>
        /// What this object will use as the second point to determine distance and scale, use is center to use this object
        /// </summary>
        [Tooltip("What this object will use as the second point to determine distance and scale, use is center to use this object")]
        public GameObject pivotPoint;
        /// <summary>
        /// boolean for if this object is the second point to determine the distance and scale
        /// </summary>
        [Tooltip("Is this object the second point to determine the distance and scale?")]
        public bool isCenter;
        /// <summary>
        /// The linear scaling past the scaleDist
        /// </summary>
        [Tooltip("The linear scaling past the scaleDist")]
        public float farScale = 0.1f;
        /// <summary>
        /// The linear scaling between the scaleDist and the pivotPoint
        /// </summary>
        [Tooltip("The linear scaling between the scaleDist and the pivotPoint")]
        public float nearScale = 0.1f;
        /// <summary>
        /// Where the object should have its original size
        /// </summary>
        [Tooltip("Where the object should have its original size")]
        public float scaleDist = 1;
        /// <summary>
        /// gets the original dimentions
        /// </summary>
        Vector3 sizeAtScaleDist;
        /// <summary>
        /// initializes the object
        /// </summary>
        void Start()
        {

            if (isCenter)
            {
                pivotPoint = this.gameObject;
            }
            if (pivotPoint == null)
            {
                Debug.LogError("No pivot point specified");
            }
            if (farScale > 1)
            {
                Debug.LogError("FarScale is incredibly large, please check the function that supplies this number");
            }
            if (nearScale > 1)
            {
                Debug.LogError("nearScale is incredibly large, please check the function that supplies this number");
            }
            //scale at scale dist is the original scale
            sizeAtScaleDist = this.transform.localScale;
        }
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            //get distance between obejcts
            float distance = (userCamera.transform.position - pivotPoint.transform.position).magnitude;
            //do the appropriate scaling
            if (distance > scaleDist)
            {
                distance = (((distance - scaleDist) * farScale) + 1);
            }
            else
            {
                distance = (((distance - scaleDist) * nearScale) + 1);
            }
            //set scale
            this.transform.localScale = distance * sizeAtScaleDist;
        }
    }
}
