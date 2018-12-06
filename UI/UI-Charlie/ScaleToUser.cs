using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScaleToUser : MonoBehaviour
{
    public GameObject Camera;
    public GameObject Center;
    public float FarScale = 1;
    public float NearScale = 1;
    public float ScaleDist = 1;
    Vector3 SizeAtScaleDist;
    public float DisapearAtCloseDistance = -1;
    public bool DisapearWhenFarAway = false;
    public float DisapearAtFarDistance;
    void Start()
    {
        if (FarScale > 1)
        {
            Debug.LogError("FarScale is increadibly large, please check the function that supplies this number");
        }
        if (NearScale > 1)
        {
            Debug.LogError("FarScale is increadibly large, please check the function that supplies this number");
        }
        if (DisapearAtCloseDistance >= 0)
        {
            if (NearScale * (ScaleDist - DisapearAtCloseDistance) > 1)
            {
                Debug.LogError("NearScale will cuase negative scale at " + (ScaleDist - (1 / NearScale)) + " pick a smaller NearScale");
            }
        }
        else
        {
            if (NearScale * ScaleDist > 1)
            {
                Debug.LogError("NearScale will cuase negative scale at "+(ScaleDist - (1/NearScale))+" pick a smaller NearScale");
            }
        }
        
        if (FarScale == null)
        {
            FarScale = 0.1f;
        }
        SizeAtScaleDist = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = (Camera.transform.position - Center.transform.position).magnitude;
        if (distance < DisapearAtCloseDistance)
        {
            this.GetComponent<Renderer>().enabled = false;
            foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }
        else if (DisapearWhenFarAway)
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
        if (distance > ScaleDist)
        {
            distance = (((distance-ScaleDist) * FarScale) + 1);
        }
        else
        {
            distance = (((distance-ScaleDist) * NearScale) + 1);
        }
        this.transform.localScale = distance * SizeAtScaleDist;
    }
}