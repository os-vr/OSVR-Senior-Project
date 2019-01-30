using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToUser : MonoBehaviour {
    [Tooltip("The camera that the user uses to see the world")]
    public GameObject userCamera;
    [Tooltip("What this object will orbit around")]
    public GameObject pivotPoint;
    [Tooltip("Distance this object will orbit around the pivotPoint")]
    public float distanceFromPivotPoint;
    Vector3 lastUp = new Vector3(0,1,0);

    // Update is called once per frame
    void FixedUpdate() {
        //vector between objects
        Vector3 direction = pivotPoint.transform.position - userCamera.transform.position;
        //up according to camera
        Vector3 up = userCamera.transform.up;
        //up is normalized to the plane of the screen
        up = (up - Vector3.Project(up, direction)).normalized;
        //andle when up is in line with direction, leading to up = 0
        if (up.magnitude != 0)
        {
            //update lastUp
            lastUp = up;
        }
        else
        {
            //set up
            up = lastUp;
        }
        //find and set rotation
        Quaternion Look = Quaternion.LookRotation(direction, up);
        transform.rotation = Look;
        //set position
        transform.position = pivotPoint.transform.position - (distanceFromPivotPoint * direction.normalized);
    }
}