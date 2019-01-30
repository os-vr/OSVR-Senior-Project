using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSwipe : Gesture {

	// Use this for initialization
	public LSwipe() : base(new ViewingNormalizer())
    {
        //setup initial region to start the L
        Check initialGoal = new PrismCheck(new Vector3(-0.1f, 1.1f, -1f), new Vector3(0.2f, 0.8f, 5f), GStatus.PASS, GStatus.CONTINUE);
        AddStage(initialGoal);

        Check midGoal = new PrismCheck(new Vector3(-0.1f, -0.1f, -1f), new Vector3(0.2f, 0.2f, 5f), GStatus.PASS, GStatus.CONTINUE);
        Check startBlock = new PrismCheck(new Vector3(0.2f, 1.1f, -1f), new Vector3(1.1f, -0.1f, 5f), GStatus.HALT, GStatus.PASS);

        CompositeCheck start = new CompositeCheck(new List<Check> { startBlock, midGoal });
        AddStage(start);

        Check endGoal = new PrismCheck(new Vector3(0.5f, 0.2f, -1f), new Vector3(0.7f, -0.1f, 5f), GStatus.PASS, GStatus.CONTINUE);
        Check midBlock = new PrismCheck(new Vector3(-0.1f, 1.1f, -1f), new Vector3(1.1f, 0.2f, 5f), GStatus.HALT, GStatus.PASS);

        CompositeCheck end = new CompositeCheck(new List<Check> { midBlock, endGoal });
        AddStage(end);

        AddEvent(new GestureEvent(delegate (GestureMetaData gmd) { AddGravity(gmd); }));


    }

    private void AddGravity(GestureMetaData gmd)
    {
        Physics.gravity = new Vector3(0.0f, -9.8f, 0.0f);
        Rigidbody[] rbs = (Rigidbody[])GameObject.FindObjectsOfType(typeof(Rigidbody));
        Debug.Log(rbs.Length);
        foreach(Rigidbody rb in rbs)
        {
            rb.useGravity = true;
        }
    }
    
}
