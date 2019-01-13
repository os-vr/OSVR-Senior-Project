using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OuchMeRibs : MonoBehaviour {
    public enum selectionType{ touch, touchAndGrab, pointAndTrigger };
    public selectionType type = selectionType.touch;
    bool inContactLeft = false;
    bool inContactRight = false;
    public delegate void pressButton(GameObject go);
    public static event pressButton pressed;

    // Use this for initialization
    void Start () {
		
	}
    void pointerPress()
    {
        if (type.Equals(selectionType.pointAndTrigger)){
            press(this.gameObject);
        }
    }
    void press(GameObject GO)
    {
        if(GO.tag.Equals("Left"))
        {
            inContactLeft = true;
        }
        else
        {
            if (GO.tag.Equals("Right"))
            {
                inContactRight = true;
            }
        }
        print("I received the message");
    }
    void press()
    {
        print("I received the message");
    }
    void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnter(collision.gameObject);
    }
    void OnCollisionEnter(GameObject go)
    {
        if (go.tag == "Right")
        {
            if (type.Equals(selectionType.touch))
            {
                press();
            }
            inContactRight = true;
        }
        else
        {
            if (go.tag == "Left")
            {
                if (type.Equals(selectionType.touch))
                {
                    press();
                }
                inContactLeft = true;
            }
        }
    }
    void onCollisionExit(Collision col)
    {
        GameObject go = col.gameObject;
        if (go.tag == "Right")
        {
            inContactRight = false;
        }
        else
        {
            if (go.tag == "Left")
            {
                inContactLeft = false;
            }
        }
    }
    void Update()
    {
        if (!type.Equals(selectionType.touchAndGrab))
        {
            return;
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) > 0)
        {
            if (inContactLeft)
            {
                press();
            }
        }
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) > 0)
        {
            if (inContactRight)
            {
                press();
            }
        }
    }
}
