using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMove : MonoBehaviour {
    private CharacterController cc;
    private Vector3 movementVector;
    private float speed = 1;
	// Use this for initialization
	void Start () {
        cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        movementVector.x = Input.GetAxis("LeftJoystickX") * speed;
        movementVector.z = Input.GetAxis("LeftJoystickY") * speed;
        movementVector.y = Input.GetAxis("RightJoystickY") * speed;
        cc.Move(movementVector * Time.deltaTime);
    }
}
