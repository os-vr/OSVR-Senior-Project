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
    public Gradient gradient;
    public float ItemFade = 1.0f;
    public UnityEvent onHover;
    float hoverTime = 0;
    private LayerMask hovering;
    private LayerMask activating;
    // Use this for initialization
    void Start () {
        triggered = false;
        dropDownControl = this.GetComponentInParent<DropDownControl>();
	}
	
	// Update is called once per frame
	void Update () {
        dropDownControl.options[index].Hovered = (Hovering - Time.time > -0.08);
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
        if (Hovering - Time.time > -0.08)
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
        if (hoverTime < 0)
        { 
            hoverTime = 0;
        }
        if (hoverTime > 1)
        {
            hoverTime = 1;
        }
        if (dropDownControl.options[index].Disabled || dropDownControl.DropDownItemGradient)
        {
            return;
        }
        nextStep();
        print("changing color");
        this.GetComponent<Renderer>().material.color = gradient.Evaluate(hoverTime);
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
        if (((1 << stay.gameObject.layer) & hovering) != 0)
        {
            if (onHover!=null)
            {
                onHover.Invoke();
            }
            Hovering = Time.time;
        }
        if (((1 << stay.gameObject.layer) & activating) != 0)
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
        if (((1 << exit.gameObject.layer) & hovering) != 0)
        {
            Hovering = -1;
            dropDownControl.options[index].Hovered = false;
        }
        if (((1 << exit.gameObject.layer) & activating) != 0)
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
    public void setHoveringlayer(LayerMask hovering)
    {
        this.hovering = hovering;
    }
    public void setActivatinglayer(LayerMask activating)
    {
        this.activating = activating;
    }
}