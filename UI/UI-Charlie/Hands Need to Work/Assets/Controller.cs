using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public bool IsLeft = false;
    LineRenderer LR;
    int layer_mask;
    // Use this for initialization
    void Start () {
		LR = GetComponent<LineRenderer>();
        layer_mask = LayerMask.GetMask("Button");
    }
	
	// Update is called once per frame
	void Update () {
        if (IsLeft) { 
            transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.Touch) > 0)
            {
                LR.enabled = true;
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) > 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 40, layer_mask))
                    {
                        GameObject Hit = hit.transform.gameObject;
                        Hit.SendMessage("pointerPress");
                    }
                }
            }
            else
            {
                LR.enabled = false;
            }
        }
        else
        {
            transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.Touch) > 0)
            {
                LR.enabled = true;
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > 0)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 40, layer_mask))
                    {
                        GameObject Hit = hit.transform.gameObject;
                        Hit.SendMessage("press");
                    }
                }
            }
            else
            {
                LR.enabled = false;
            }
        }
    }
}
