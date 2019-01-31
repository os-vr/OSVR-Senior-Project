using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropDownItem : MonoBehaviour {
    public DropDownControl dropDownControl;
    public GameObject Extra;
    public string label;
    public int index;
    public delegate void chosen(int choose);
    public static event chosen choose;
    public class picked : UnityEvent<int> { };
    public picked onValueChanged = new picked();
    bool triggered;
    float Hovering = -1;
    public Gradient grad;
    public UnityEvent onHover;
    float hoverTime = 0;
	// Use this for initialization
	void Start () {
        triggered = false;
        dropDownControl = this.GetComponentInParent<DropDownControl>();
	}
	
	// Update is called once per frame
	void Update () {
        if (dropDownControl.options[index].Disabled || !dropDownControl.DropDownItemGradient)
        {
            return;
        }
        if (Hovering - Time.time > -0.08)
        {
            hoverTime += Time.deltaTime * 8;
        }
        if (hoverTime > 1)
        {
            hoverTime = 1;
        }
        this.GetComponent<Renderer>().material.color = grad.Evaluate(hoverTime);
            if(hoverTime > 0)
            {
                hoverTime -= Time.deltaTime * 4;
            }
	}
    void onTriggerEnter(Collider enter)
    {
        if (dropDownControl.interactable)
        {
            OnTriggerStay(enter);
        }
    }
    void OnTriggerStay(Collider stay)
    {
        if (!dropDownControl.interactable)
        {
            return;
        }
        if (stay.gameObject.layer == LayerMask.NameToLayer("Hovering"))
        {
            dropDownControl.options[index].Hovered = true;
            if (onHover!=null)
            {
                onHover.Invoke();
            }
            Hovering = Time.time;
        }
        if (stay.gameObject.layer == LayerMask.NameToLayer("Activating"))
        {
            if (!triggered)
            {
                dropDownControl.options[index].Pressed = true;
                triggered = true;
                /*
                int counter = 0;
                for (int i = 0; i < dropDownControl.options.Length; i++)
                {
                    
                    string op = dropDownControl.options[i].label;
                    if (op == label)
                    {
                        onValueChanged.Invoke(counter);
                        break;
                    }
                    counter++;
                }
                */
                onValueChanged.Invoke(index);
            }
        }
    }
    void OnTriggerExit(Collider exit)
    {
        if (!dropDownControl.interactable)
        {
            return;
        }
        if (exit.gameObject.layer == LayerMask.NameToLayer("Hovering"))
        {
            //onExit.Invoke();
            Hovering = -1;
            dropDownControl.options[index].Hovered = false;
        }
        if (exit.gameObject.layer == LayerMask.NameToLayer("Activating"))
        {
            triggered = false;
            Hovering = -1;
            dropDownControl.options[index].Hovered = false;
        }
    }
    void onDisable()
    {
        Destroy(Extra);
        Extra = Instantiate(dropDownControl.options[index].extrasDisabled,this.transform.position, this.transform.rotation);
    }
    void onEnable()
    {
        Destroy(Extra);
        nextStep();
    }
    void nextStep()
    {
        if (dropDownControl.options[index].Hovered)
        {
            Extra = Instantiate(dropDownControl.options[index].extrasHovered, this.transform.position, this.transform.rotation);
        }
        else
        {
            if (dropDownControl.options[index].Pressed)
            {
                Extra = Instantiate(dropDownControl.options[index].extrasPressed, this.transform.position, this.transform.rotation);
            }
            else
            {
                Extra = Instantiate(dropDownControl.options[index].extras, this.transform.position, this.transform.rotation);
            }
        }
    }
}
