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
        dropDownControl.options[index].Hovered = (Hovering - Time.time > -0.08);
        if (Hovering - Time.time > -0.08)
        {
            hoverTime += Time.deltaTime * 8;
        }
        if (hoverTime > 0)
        {
            hoverTime -= Time.deltaTime * 4;
        }
        if (hoverTime < 0)
        { 
            hoverTime = 0;
        }
        if (hoverTime > 1)
        {
            hoverTime = 1;
        }
        if (dropDownControl.options[index].Disabled || !dropDownControl.DropDownItemGradient)
        {
            return;
        }
        nextStep();
        this.GetComponent<Renderer>().material.color = grad.Evaluate(hoverTime);
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
                Destroy(Extra);
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
    public void setEnabled(bool value)
    {
        if (value)
        {
            onEnable();
        }
        else
        {
            onDisable();
        }
    }
    void onDisable()
    {
        Destroy(Extra);
        if (dropDownControl.options[index].extrasDisabled != null)
        {
            Extra = Instantiate(dropDownControl.options[index].extrasDisabled, this.transform.position, this.transform.rotation);
            Extra.transform.parent = this.gameObject.transform;
        }
    }
    void onEnable()
    {
        nextStep();
    }
    void nextStep()
    {
        Destroy(Extra);
        if (dropDownControl.options[index].Hovered)
        {
            if (dropDownControl.options[index].extrasHovered != null)
            {
                Extra = Instantiate(dropDownControl.options[index].extrasHovered, this.transform.position, this.transform.rotation);
                Extra.transform.parent = this.gameObject.transform;
            }
        }
        else
        {
            if (dropDownControl.options[index].Pressed)
            {
                if (dropDownControl.options[index].extrasPressed != null)
                {
                    Extra = Instantiate(dropDownControl.options[index].extrasPressed, this.transform.position, this.transform.rotation);
                    Extra.transform.parent = this.gameObject.transform;
                }
            }
            else
            {
                if (dropDownControl.options[index].extras != null)
                {
                    Extra = Instantiate(dropDownControl.options[index].extras, this.transform.position, this.transform.rotation);
                    Extra.transform.parent = this.gameObject.transform;
                }
            }
        }
    }
}
