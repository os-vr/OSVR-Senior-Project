using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearAtDistance : MonoBehaviour {
    [Tooltip("The camera that the user uses to see the world")]
    public GameObject userCamera;
    [Tooltip("What this object will use as the second point to determine distance and scale, use is center to use this object")]
    public GameObject pivotPoint;
    [Tooltip("Is this object the second point to determine the distance and scale?")]
    public bool isCenter;
    [Tooltip("Where the object should have its original size")]
    public float scaleDist = 1;
    [Tooltip("How close the user should be when this item disappears")]
    public float disapearAtCloseDistance = -1;
    [Tooltip("Should this item disappear if the user gets to far away?")]
    public bool disapearWhenFarAway = false;
    [Tooltip("The distance at which this item should disapear")]
    public float disapearAtFarDistance = 1000;
    [Tooltip("Renderers that will turn on or off as stated by the script")]
    public ArrayList renderList = new ArrayList();
    [Tooltip("Use all renderers in this item")]
    public bool itemRenderer = true;
    [Tooltip("Use all renderers in children")]
    public bool childrenRenderer = true;
    [Tooltip("Is this item visible?")]
    public bool isVisible = true;
    Vector3 sizeAtScaleDist;
    // Use this for initialization
    void Start () {
        if (childrenRenderer)
        {
            foreach (Renderer r in this.gameObject.GetComponentsInChildren<Renderer>())
            {
                renderList.Add(r);
            }
            if (!itemRenderer)
            {
                renderList.Remove(this.gameObject.GetComponent<Renderer>());
            }
        }
        else
        {
            if (itemRenderer)
            {
                renderList.Add(this.gameObject.GetComponent<Renderer>());
            }
            else
            {
                Debug.Log("None of the renderers are requested to be assigned, so this means the item will not disappear. Set the disappearAtNearDistance to a negative distance and turn DisappearAtFarDistance to false instead");
            }
        }
        if (renderList.Count == 0 && (itemRenderer || childrenRenderer))
        {
            Debug.Log("No renderers are selected dispite the fact that the options were selected. Check if the renderers are in the right place.");
        }
    }
    ///Add renderer to the list of renderers to turn on and off
    void addRenderer(Renderer r)
    {
        renderList.Add(r);
        r.enabled = isVisible;
    }
    ///Remove renderer to the list of renderers to turn on and off
    void removeRenderer(Renderer r)
    {
        renderList.Remove(r);
    }
    // Update is called once per frame
    void Update () {
        //get distance between obejcts
        float distance = (userCamera.transform.position - pivotPoint.transform.position).magnitude;
        print(distance);
        if (distance < disapearAtCloseDistance)
        {
            //check if you need to set visibility
            if (isVisible)
            {
                //set visibility
                isVisible = false;
                foreach (Renderer r in renderList)
                {
                    r.enabled = false;
                }
            }
            //check if you are not close enough to be invisible
        }
        else
        {
            //check if you need to check for invisibility at distance
            if (disapearWhenFarAway)
            {
                //check if you are far enough to be invisible
                if (distance > disapearAtFarDistance)
                {
                    //check if you need to change visibility
                    if (isVisible)
                    {
                        //set visibility
                        isVisible = false;
                        foreach (Renderer r in renderList)
                        {
                            r.enabled = false;
                        }
                    }
                    //if close enough to be visibile
                }
                else
                {
                    //check if visibility needs to be changed
                    if (!isVisible)
                    {
                        //set visibility
                        isVisible = true;
                        foreach (Renderer r in renderList)
                        {
                            r.enabled = true;
                        }
                    }
                }
                //if not close enough to be invisible and doesn't disapear at range, then it is visibile
            }
            else
            {
                //check if you need to set visibility
                if (!isVisible)
                {
                    //set visibility
                    isVisible = true;
                    foreach (Renderer r in renderList)
                    {
                        r.enabled = true;
                    }
                }
            }
        }
    }
}
