using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour {
    public bool onEnterUseForHover = false;
    public UnityEvent onEnter;
    public bool onStayUseForHover = false;
    public UnityEvent onStay;
    public bool onExitUseForHover = false;
    public UnityEvent onExit;
    
    public bool detectOnEnter = true; 
    public bool detectOnStay = true;
    public bool detectOnExit = true;
    public LayerMask activationLayer;
    public LayerMask hoverLayer;

    // Use this for initialization
    void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //If a selector collides with the button, button is set to true

    private void OnTriggerEnter(Collider other)
    {
        if (detectOnEnter)
        {

            //LayerMask value is 2 to the power of its layer index
            if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && !onEnterUseForHover)
            {
                Debug.Log("Act Layer Enter");
                onEnter.Invoke();
            }

            else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onEnterUseForHover)
            {
                Debug.Log("Hover Layer Enter");
                onEnter.Invoke();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (detectOnStay)
        {
            //LayerMask value is 2 to the power of its layer index
            if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && !onStayUseForHover)
            {
                Debug.Log("Act Layer Stay");
                onStay.Invoke();
            }

            else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onStayUseForHover)
            {
                Debug.Log("Hover Layer Stay");
                onStay.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (detectOnExit)
        {
            //LayerMask value is 2 to the power of its layer index
            if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && !onExitUseForHover)
            {
                Debug.Log("Act Layer Exit");
                onExit.Invoke();
            }

            else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onExitUseForHover)
            {
                Debug.Log("Hover Layer Exit");
                onExit.Invoke();
            }
        }
    }
}
