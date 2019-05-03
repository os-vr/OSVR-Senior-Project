using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSVR.UI
{
/// <summary>
/// controls the visibility of text based on distance, both close and long range.
/// </summary>
/// <remarks>
/// ensures the screen is not cluttered since most items you care about must be within a certian range to be relevant.
/// </remarks>
    public class DisappearAtDistance : MonoBehaviour
    {
        /// <summary>
        /// Camera for doing vector calculations. The camera that the user uses to see the world
        /// </summary>
        [Tooltip("The camera that the user uses to see the world")]
        public GameObject userCamera;
        /// <summary>
        /// What this object will use as the second point to determine distance and scale, use is center to use this object
        /// </summary>
        [Tooltip("What this object will use as the second point to determine distance and scale, use is center to use this object")]
        public GameObject pivotPoint;
        /// <summary>
        /// Is this object the second point to determine the distance and scale?
        /// </summary>
        [Tooltip("Is this object the second point to determine the distance and scale?")]
        public bool isCenter;
        /// <summary>
        /// How close the user should be when this item disappears
        /// </summary>
        [Tooltip("How close the user should be when this item disappears")]
        public float disappearAtCloseDistance = -1;
        /// <summary>
        /// Should this item disappear if the user gets to far away?
        /// </summary>
        [Tooltip("Should this item disappear if the user gets to far away?")]
        public bool disappearWhenFarAway = false;
        /// <summary>
        /// The distance at which this item should disapear
        /// </summary>
        [Tooltip("The distance at which this item should disappear")]
        public float disappearAtFarDistance = 1000;

        /// <summary>
        /// Renderers that will turn on or off as stated by the script
        /// </summary>
        [Tooltip("Renderers that will turn on or off as stated by the script")]
        public ArrayList renderList = new ArrayList();
        /// <summary>
        /// Use all renderers in this item
        /// </summary>
        [Tooltip("Use all renderers in this item")]
        public bool itemRenderer = true;
        /// <summary>
        /// Use all renderers in children
        /// </summary>
        [Tooltip("Use all renderers in children")]
        public bool childrenRenderer = true;
        /// <summary>
        /// Is this item visible?
        /// </summary>
        [Tooltip("Is this item visible?")]
        public bool isVisible = true;
        /// <summary>
        /// use this to know how large items whould be at the scale distance 
        /// </summary>
        Vector3 sizeAtScaleDist;
        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Start()
        {
            //handle renderers in children
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
        /// <summary>
        /// Add renderer to the list of renderers to turn on and off
        /// </summary>
        /// <param name="r"> the renderer you need to add</param>
        void addRenderer(Renderer r)
        {
            renderList.Add(r);
            r.enabled = isVisible;
        }
        /// <summary>
        /// Remove renderer to the list of renderers to turn on and off
        /// </summary>
        /// <param name="r"> the renderer you need to remove</param>
        void removeRenderer(Renderer r)
        {
            renderList.Remove(r);
        }
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            //get distance between obejcts
            float distance = (userCamera.transform.position - pivotPoint.transform.position).magnitude;
            print(distance);
            if (distance < disappearAtCloseDistance)
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
                if (disappearWhenFarAway)
                {
                    //check if you are far enough to be invisible
                    if (distance > disappearAtFarDistance)
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
                    //if not close enough to be invisible and doesn't disappear at range, then it is visibile
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
}
