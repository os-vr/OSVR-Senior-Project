using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
    public GameObject selector;
    public GameObject button;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //If a selector collides with the button, button is set to true
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == selector)
        {
            button.SetActive(true);
        }
    }
}
