
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OSVR.UI
{
/// <summary>
/// ensures the items are turned in the right direction, intended to keep text readable at all angles.
/// </summary>
    public class RotateToUser : MonoBehaviour
    {
        /// <summary>
        /// The camera that the user uses to see the world
        /// </summary>
        [Tooltip("The camera that the user uses to see the world")]
        public GameObject userCamera;
        /// <summary>
        /// What this object will orbit around
        /// </summary>
        [Tooltip("What this object will orbit around")]
        public GameObject pivotPoint;
        /// <summary>
        /// boolean for if this object is the second point to determine the distance and scale
        /// </summary>
        [Tooltip("Is this object the second point to determine the rotation?")]
        public bool isCenter;
        /// <summary>
        /// Distance this object will orbit around the pivotPoint
        /// </summary>
        [Tooltip("Distance this object will orbit around the pivotPoint")]
        public float distanceFromPivotPoint;
        /// <summary>
        /// used when there is no valid up, updates when applicable.
        /// </summary>
        Vector3 lastUp = new Vector3(0, 1, 0);
        /// <summary>
        /// FixedUpdate is called once per frame
        /// </summary>
        void Start()
        {
            if (isCenter)
            {
                pivotPoint = this.gameObject;
            }
            if (pivotPoint == null)
            {
                Debug.LogError("No pivot point specified");
            }
        }
        void FixedUpdate()
        {
            //vector between objects
            Vector3 direction = pivotPoint.transform.position - userCamera.transform.position;
            //up according to camera
            Vector3 up = userCamera.transform.up;
            //up is normalized to the plane of the screen
            up = (up - Vector3.Project(up, direction)).normalized;
            //angle when up is in line with direction, leading to up = 0
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
}
