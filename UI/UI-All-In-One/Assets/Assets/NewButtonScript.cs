using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewButtonScript : MonoBehaviour
{
   
    GameObject extra;
    bool triggered;
    float hovering = -1;
    public Gradient gradient;
    public bool useGradient;
    public Color startColor;
    public Color endColor;
    public float ItemFade = 1.0f;
    public float delay = -0.08f;
    float hoverTime = 0;

    public bool onEnterUseForHover = false;
    public bool onEnterUseForActivation = false;
    public UnityEvent onEnter = new UnityEvent();

    public bool onStayUseForHover = false;
    public bool onStayUseForActivation = false;
    public UnityEvent onHover = new UnityEvent();

    public bool onExitUseForHover = false;
    public bool onExitUseForActivation = false;
    public UnityEvent onExit = new UnityEvent();

    public UnityEvent onPress = new UnityEvent();

    public bool detectOnEnter = true; 
    public bool detectOnHover = true;
    public bool detectOnExit = true;
    public LayerMask hoverLayer;
    public LayerMask activationLayer;
    
    public bool hovered;
    public bool pressed;
    public bool disabled;
    public GameObject extras;
    public GameObject extrasHovered;
    public GameObject extrasPressed;
    public GameObject extrasDisabled;
    // Use this for initialization
    void Start()
    {
        triggered = false;
        if(startColor == null)
        {
            startColor = this.GetComponent<Renderer>().material.color;
        }
        if (extras != null)
        {
            extra = Instantiate(extras);
        }
    }

    // Update is called once per frame
    void Update()
    {
        hovered = ((hovering - Time.time) > delay);
        if (hoverTime > 0)
        {
            if (ItemFade == 0)
            {
                hoverTime = 1;
            }
            else
            {
                hoverTime -= Time.deltaTime * (1 / ItemFade);
            }
        }
        if ((hovering - Time.time) > delay)
        {
            if (ItemFade == 0)
            {
                hoverTime = 1;
            }
            else
            {
                hoverTime += Time.deltaTime * 2 * (1 / ItemFade);
            }
        }
        else
        {
            if (ItemFade == 0)
            {
                hoverTime = 0;
            }
        }
        if (hoverTime < 0)
        {
            hoverTime = 0;
        }
        if (hoverTime > 1)
        {
            hoverTime = 1;
        }
        if (disabled)
        {
            return;
        }
        nextStep();
        if (!useGradient)
        {
            if (hoverTime == 1) {
                if (endColor != null)
                {
                    this.GetComponent<Renderer>().material.color = endColor;
                }
            }
            else
            {
                this.GetComponent<Renderer>().material.color = startColor;
            }
            return;
        }
        
        print("changing color");
        this.GetComponent<Renderer>().material.color = gradient.Evaluate(hoverTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (detectOnEnter && !disabled)
        {

            //LayerMask value is 2 to the power of its layer index
            if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value)
            {
                Debug.Log("Act Layer Enter");
                pressed = true;
                if (onEnter != null && onEnterUseForActivation)
                {
                    onEnter.Invoke();
                }
                if (onPress != null)
                {
                    onPress.Invoke();
                }
            }

            else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onEnterUseForHover)
            {
                Debug.Log("Hover Layer Enter");
                if (onEnter != null)
                {
                    onEnter.Invoke();
                }
            }

            else if (onEnterUseForActivation && onEnterUseForHover)
            {
                Debug.Log("Both Layer Enter");
                pressed = true;
                if (onEnter != null)
                {
                    onEnter.Invoke();
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
       if (detectOnHover && !disabled)
        {

            hovered = true;

            if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value)
            {
                Debug.Log("Act Layer Stay");
                pressed = true;
                if (onHover != null && onStayUseForActivation)
                {
                    onHover.Invoke();

                }
                if (onPress != null)
                {
                    onPress.Invoke();
                }
                hovering = Time.time;
            }

            else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onStayUseForHover)
            {
                Debug.Log("Hover Layer Stay");
                if (onHover != null)
                {
                    onHover.Invoke();
                }
                hovering = Time.time;
            }
            else if (onStayUseForActivation && onStayUseForHover)
            {
                Debug.Log("Both Layer Enter");
                pressed = true;
                if (onHover != null)
                {
                    onHover.Invoke();
                }
                hovering = Time.time;
            }
        }
       
    }
    void OnTriggerExit(Collider other)
    {
        if (detectOnExit && !disabled)
        {
            if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && onExitUseForActivation)
            {
                Debug.Log("Act Layer Exit");
                pressed = true;
                if (onExit != null)
                {
                    onExit.Invoke();
                }
                hovering = -1;
            }

            else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onExitUseForHover)
            {
                Debug.Log("Hover Layer Exit");
                if (onExit != null)
                {
                    onExit.Invoke();
                }
                hovering = -1;
            }
            else if (onExitUseForActivation && onExitUseForHover)
            {
                Debug.Log("Both Layer Enter");
                pressed = true;
                if (onExit != null)
                {
                    onExit.Invoke();
                }
                hovering = -1;
            }
        }
    }

    public void setEnabled(bool value)
    {
        if (value)
        {
            if (disabled)
            {
                onEnable();
            }
        }
        else
        {
            if (!disabled)
            {
                onDisable();
            }
        }
    }
    void onDisable()
    {
        if (extra != null)
        {
            Destroy(extra);
        }
        if (extrasDisabled != null)
        {
            extra = Instantiate(extrasDisabled, this.transform.position, this.transform.rotation);
            extra.transform.parent = this.gameObject.transform;
        }
    }
    void onEnable()
    {
        nextStep();
    }
    void nextStep()
    {
        Destroy(extra);
        if (hovered)
        {
            Debug.Log("What's up");
            if (extrasHovered != null)
            {
                extra = Instantiate(extrasHovered, this.transform.position, this.transform.rotation);
                extra.transform.parent = this.gameObject.transform;
            }
        }
        else
        {
            if (pressed)
            {
                if (extrasPressed != null)
                {
                    extra = Instantiate(extrasPressed, this.transform.position, this.transform.rotation);
                    extra.transform.parent = this.gameObject.transform;
                }
            }
            else
            {
                if (extras != null)
                {
                    extra = Instantiate(extras, this.transform.position, this.transform.rotation);
                    extra.transform.parent = this.gameObject.transform;
                }
            }
        }
    }
    public void setHovered(bool state)
    {
        hovered = state;
        nextStep();
    }
    public void setPressed(bool state)
    {
        pressed = state;
        nextStep();
    }
    public void setDisabled(bool state)
    {
        disabled = state;
        nextStep();
    }
    public void setHoveringlayer(LayerMask hovering)
    {
        this.hoverLayer = hovering;
    }
    public void setActivatinglayer(LayerMask activating)
    {
        this.activationLayer = activating;
    }
}
