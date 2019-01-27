using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLikeChild : MonoBehaviour {
    Transform T;
    // Use this for initialization
    void Start () {
        T = GameObject.Find("Main Camera").transform;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        this.transform.rotation = Quaternion.LookRotation(T.forward, T.up);
	}
}
