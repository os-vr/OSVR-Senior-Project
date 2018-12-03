using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToUser : MonoBehaviour {
    public GameObject Camera;
    public float Scale = 1;
    public float SizeAtOneMeter;
    public float DisapearAtCloseDistance = -1;
    public bool DisapearWhenFarAway = false;
    public float DisapearAtFarDistance;
    void Start () {
        if (Scale > 1000000)
        {
            Debug.LogError("Scale is increadibly large, please check the function that supplies this number");
        }
        if (Scale < 0)
        {
            Debug.LogError("Scale is negative, please use a positve number");
        }
        Scale = 1;
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
        //print(this.transform.lossyScale);
        //this.transform.localScale = (distance) * this.transform.lossyScale;
    }
}