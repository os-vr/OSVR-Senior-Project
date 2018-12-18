using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScaleToUser : MonoBehaviour
{
    [Tooltip("The camera that the user uses to see the world")]
    public GameObject userCamera;
    [Tooltip("What this object will use as the second point to determine distance and scale, use is center to use this object")]
    public GameObject pivotPoint;
    [Tooltip("Is this object the second point to determine the distance and scale?")]
    public bool isCenter;
    [Tooltip("The linear scaling past the scaleDist")]
    public float farScale = 0.1f;
    [Tooltip("The linear scaling between the scaleDist and the pivotPoint")]
    public float nearScale = 0.1f;
    [Tooltip("Where the object should have its original size")]
    public float scaleDist = 1;
    Vector3 sizeAtScaleDist;
    void Start()
    {
        
        if (isCenter)
        {
            pivotPoint = this.gameObject;
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

    // Update is called once per frame
    void Update(){
        //get distance between obejcts
        float distance = (userCamera.transform.position - pivotPoint.transform.position).magnitude;
        //do the appropriate scaling
        if (distance > scaleDist)
        {
            distance = (((distance-scaleDist) * farScale) + 1);
        }
        else
        {
            distance = (((distance-scaleDist) * nearScale) + 1);
        }
        //set scale
        this.transform.localScale = distance * sizeAtScaleDist;
    }
}