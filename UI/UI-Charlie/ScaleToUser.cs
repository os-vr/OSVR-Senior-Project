using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScaleToUser : MonoBehaviour {
    public GameObject Camera;
    public float FarScale = 1;
    public float NearScale = 1;
    Vector3 SizeAtScaleDist;
    public float DisapearAtCloseDistance = -1;
    public bool DisapearWhenFarAway = false;
    public float DisapearAtFarDistance;
    void Start () {
        if (FarScale > 1000000)
        {
            Debug.LogError("Scale is increadibly large, please check the function that supplies this number");
        }
        if (FarScale <= 1)
        {
            Debug.LogError("Scale is to low, please use a higher number");
        }
        if (FarScale == null)
        {
            FarScale = 1;
        }
        SizeAtScaleDist = this.transform.lossyScale;
    }
	
	// Update is called once per frame
	void Update () {
        float distance = (Camera.transform.position - this.transform.position).magnitude;
        if (distance < DisapearAtCloseDistance)
        {
            this.GetComponent<Renderer>().enabled = false;
            foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }else if (DisapearWhenFarAway)
        {
            if (DisapearAtFarDistance < distance)
            {
                this.GetComponent<Renderer>().enabled = false;
                foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = false;
                }
            }
        }
        else
        {
            this.GetComponent<Renderer>().enabled = true;
            foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
            {
                r.enabled = true;
            }
        }
        if (distance < FarScale)
        {
            distance = (float)System.Math.Log(distance, NearScale);
        }
        else
        {
            distance = (float)System.Math.Log(distance, FarScale);
        }
        this.transform.localScale = distance * SizeAtScaleDist;
    }
}