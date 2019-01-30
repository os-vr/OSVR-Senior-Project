using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForth : MonoBehaviour {
    float StartTime;
    float Direction = 1;
    public float speed = 1;
    public int SwayTime = 3;
    GameObject Camera;
	// Use this for initialization
	void Start () {
        StartTime = Time.time;
        Camera = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time-StartTime > SwayTime)
        {
            Direction *= -1;
            StartTime = Time.time;
        }
        this.transform.position = this.transform.position + Camera.transform.forward * (speed * Time.deltaTime * Direction);
	}
}
