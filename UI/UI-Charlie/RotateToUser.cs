using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToUser : MonoBehaviour {
    public GameObject Camera;
    public GameObject PivotPoint;
    public float distanceFromObject;
    Vector3 LastUp = new Vector3(0,1,0);

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 direction = PivotPoint.transform.position - Camera.transform.position;
        Vector3 up = Camera.transform.up;
        up = (up - Vector3.Project(up, direction)).normalized;
        if (up.magnitude != 0)
        {
            LastUp = up;
        }
        else
        {
            up = LastUp;
        }
        Quaternion Look = Quaternion.LookRotation(direction, Camera.transform.up);
        transform.rotation = Look;
        transform.position = PivotPoint.transform.position - (distanceFromObject * direction.normalized);
    }
}
