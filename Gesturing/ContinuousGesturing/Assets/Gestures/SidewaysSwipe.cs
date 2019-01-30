using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SidewaysSwipe : Gesture {
    
    int sign;
	// Use this for initialization
	public SidewaysSwipe(bool rightward) : base(new ViewingNormalizer())
    {
        Check start = new PrismCheck(new Vector3(-0.1f, -0.1f, -0.1f), new Vector3(0.1f, 1.1f, 5f), GStatus.PASS, GStatus.CONTINUE);
        Check end = new PrismCheck(new Vector3(0.9f, -0.1f, -0.1f), new Vector3(1.1f, 1.1f, 5f), GStatus.PASS, GStatus.CONTINUE);

        
        
        if (rightward)
        {
            AddStage(start);
            AddStage(end);
            sign = 1;
        }
        else {
            AddStage(end);
            AddStage(start);
            sign = -1;
        }
        
        AddEvent(new GestureEvent(delegate (GestureMetaData gmd) { InstantiateRigidbody(gmd); }));
    }

    private void InstantiateRigidbody(GestureMetaData gmd)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.AddComponent<Rigidbody>();
        Vector3 velocity = Random.insideUnitCircle;
        Debug.Log(velocity);
        obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        obj.GetComponent<Rigidbody>().velocity = velocity;
        obj.GetComponent<Rigidbody>().useGravity = false;
    }


    
}
