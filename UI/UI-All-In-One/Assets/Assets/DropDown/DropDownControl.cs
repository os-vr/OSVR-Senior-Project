using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class DropDownControl : MonoBehaviour
{
    [System.Serializable]
    public class Option
    {
        [SerializeField]
        public string label;
        [SerializeField]
        public GameObject extras;
        [SerializeField]
        public GameObject extrasHovered;
        [SerializeField]
        public GameObject extrasPressed;
        [SerializeField]
        public GameObject extrasDisabled;
        [SerializeField]
        public bool Pressed;
        [SerializeField]
        public bool Disabled;
        [SerializeField]
        public bool Hovered;
        [SerializeField]
        public Gradient buttonGrad;
        public Option()
        {

        }
    }
    public bool interactable = true;
    private bool interactable_past = true;
    private bool Interactable = true;
    public bool interactable_ {
        get{
            return Interactable;
        }
        set
        {
            Interactable = value;
            foreach (Option op in options)
            {
                op.Disabled = !value;
            }
            if (scrollingOptions.Length == 0)
            {
                return;
            }
            if (scrollingOptions[0])
            {
                foreach (GameObject op in scrollingOptions)
                {
                    op.GetComponentInChildren<DropDownItem>().setEnabled(value);
                }
            }
        }
    }
    [Tooltip("The sub item that will be created in the dropdown")]
    public GameObject template;
    [Tooltip("The difference in the heights of the item when drawn")]
    public float changeInHeight;
    [Tooltip("The item that will surround all the items in the dropdown")]
    public GameObject boundingBox;
    [Tooltip("The option that is selected if there is no input")]
    public string defaultString = "default";
    [HideInInspector]
    [Tooltip("where in the list the item will appear in the drop down. If it will not appear in the dropdown, set to -1")]
    public int defaultnumber = -1;
    [Tooltip("How many options you can display")]
    public int maxOptionsShown = 5;
    [Tooltip("Old implementation of sliders for testing")]
    public Slider oldSlider;
    [Tooltip("Affects when an item will be faded out in the slider implementation")]
    public float hangpercentage = 0.5f;
    [Tooltip("Will the dropdown box use gradient or switch implementation")]
    public bool DropDownBoxGradient = true;
    [Tooltip("Will the dropdown items use gradient or switch implementation")]
    public bool DropDownItemGradient = true;
    [Tooltip("gradient for the box")]
    public Gradient grad;
    [Tooltip("gradient for the items")]
    public Gradient buttonGrad;
    float Hovering = -1;
    float hoverTime = 0;
    float slider = 0;
    GameObject[] scrollingOptions;
    [HideInInspector]
    public TextMeshPro label;
    public UnityEvent onDropClick;
    public UnityEvent onSelectClick;
    public UnityEvent onHover;
    public UnityEvent onEnter;
    public UnityEvent onExit;
    //public class valueChangeEvent : UnityEvent<int> { }
    //public valueChangeEvent onValueChange;
    bool activated;
    [System.Serializable]
    public class onValueChange : UnityEvent<int> { };
    [SerializeField]
    public onValueChange onValueChanged = new onValueChange();
    [SerializeField]
    public Option[] options;
    public float last_activation = -1;
    void Awake()
    {
        maxOptionsShown = options.Length;
        this.label = this.GetComponentInChildren<TextMeshPro>();
        label.text = defaultString;
        if (oldSlider != null)
        {
            oldSlider.gameObject.active = false;
        }
        if (defaultnumber < 0)
        {
            label.text = defaultString;
        }
        else
        {
            if (defaultString.Length == 0)
            {
                label.text = options[defaultnumber].label;
            }
            else
            {
                string[] temp = new string[options.Length + 1];
                int counter = 0;
                for (int i = 0; i < options.Length; i++)
                {
                    string op = options[i].label;
                    if (counter == defaultnumber)
                    {
                        temp[counter] = defaultString;
                        counter++;
                    }
                    temp[counter] = op;
                    counter++;
                }
                if (counter == defaultnumber)
                {
                    temp[counter] = defaultString;
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        label = this.gameObject.GetComponentInChildren<TextMeshPro>();
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(interactable != interactable_past)
        {
            interactable_ = interactable;
            interactable_past = interactable;
        }
        if (DropDownBoxGradient)
        {
            if (Hovering - Time.time > -0.08)
            {
                hoverTime += Time.deltaTime * 8;
            }
            if (hoverTime > 0)
            {
                hoverTime -= Time.deltaTime * 4;
            }
            if (hoverTime > 1)
            {
                hoverTime = 1;
            }
            this.GetComponent<Renderer>().material.color = grad.Evaluate(hoverTime);
        }
    }
    void deactivate_dropdown()
    {
        activated = false;
        foreach (Transform t in this.transform)
        {
            if (t.gameObject.name != "Label")
            {
                Destroy(t.gameObject);
            }
        }
    }
    void activate_dropdown()
    {
        if (activated == false && interactable_)
        {
            activated = true;
            int position = 2;
            GameObject BoundingBox = Instantiate(boundingBox, this.transform.position, this.transform.rotation);
            BoundingBox.transform.SetParent(this.transform);
            bool scrolling = false;
            scrollingOptions = new GameObject[options.Length];
            if (maxOptionsShown < options.Length)
            {
                scrolling = true;
            }
            for (int i = 0; i < options.Length; i++)
            {
                string op = options[i].label;
                GameObject inst = Instantiate(template, this.transform.position + this.transform.up * -position * changeInHeight, this.transform.rotation);
                GameObject extra = null;
                Option opt = options[i];
                if (opt.Disabled)
                {
                    if (opt.extrasDisabled != null)
                    {
                        extra = Instantiate(opt.extrasDisabled, this.transform.position + this.transform.up * -position * changeInHeight, this.transform.rotation);
                    }
                }
                else
                {
                    if (opt.Hovered)
                    {
                        if (opt.extrasHovered != null)
                        {
                            extra = Instantiate(opt.extrasHovered, this.transform.position + this.transform.up * -position * changeInHeight, this.transform.rotation);
                        }
                    }
                    else
                    {
                        if (opt.Pressed)
                        {
                            if (opt.extrasPressed != null)
                            {
                                extra = Instantiate(opt.extrasPressed, this.transform.position + this.transform.up * -position * changeInHeight, this.transform.rotation);
                            }

                        }
                        else
                        {
                            if (opt.extras != null)
                            {
                                extra = Instantiate(opt.extras, this.transform.position + this.transform.up * -position * changeInHeight, this.transform.rotation);
                            }
                        }
                    }
                }
                DropDownItem DDI = inst.gameObject.GetComponentInChildren<Collider>().gameObject.AddComponent<DropDownItem>() as DropDownItem;
                if (extra != null)
                {
                    extra.transform.parent = inst.transform;

                }
                DDI.index = i;
                DDI.Extra = extra;
                DDI.label = op;
                DDI.onValueChanged.AddListener(closeAndPick);
                DDI.dropDownControl = this;
                inst.transform.SetParent(this.transform);
                TextMeshPro text = inst.GetComponentInChildren<TextMeshPro>();
                text.text = op;
                if (DropDownItemGradient)
                {
                    DDI.grad = buttonGrad;
                }
                else
                {
                    DDI.grad = options[i].buttonGrad;
                }
                position++;
                if (i > maxOptionsShown - 1)
                {
                    inst.SetActive(false);
                }
                scrollingOptions[i] = inst;
            }
            if (scrolling)
            {
                //create new slider
                oldSlider.gameObject.active = true;
                //oldSlider.transform.position = this.transform.position + /*this.transform.up*/new Vector3(1, 0, 0);
                //add listener on the slider float event
                oldSlider.onValueChanged.AddListener(delegate { SliderUpdate(); });
            }
        }
    }
    public void SliderUpdate()
    {
        if (!interactable_)
        {
            return;
        }
        float value = oldSlider.value;
        float diff = (scrollingOptions.Length - maxOptionsShown) * changeInHeight;
        int first = (int)((float)(value * diff) / (float)(changeInHeight));
        int hang = 0;
        if (((float)(value * diff) / (float)(changeInHeight)) % 1 > hangpercentage)
        {
            hang = 1;
        }
        for (int i = 0; i < scrollingOptions.Length; i++)
        {
            GameObject go = scrollingOptions[i];
            go.SetActive(false);
        }
        for (int i = first; i < first + hang + maxOptionsShown; i++)
        {
            if (i > scrollingOptions.Length - 1)
            {
                break;
            }
            GameObject go = scrollingOptions[i];
            go.SetActive(true);
            go.transform.position = this.transform.position + /*this.transform.up*/new Vector3(0, 1, 0) * (((-i - 2) * changeInHeight) + (diff * value));
            go.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider enter)
    {
        if (interactable_)
        {
            OnTriggerStay(enter);
        }
    }
    void OnTriggerStay(Collider enter)
    {
        if (!interactable_)
        {
            return;
        }
        if (enter.gameObject.layer == LayerMask.NameToLayer("Hovering"))
        {
            if (onHover != null)
            {
                onHover.Invoke();
            }
        }
        if (enter.gameObject.layer == LayerMask.NameToLayer("Activating"))
        {
            if (Time.time - last_activation < 0.08)
            {
                last_activation = Time.time;
            }
            else
            {
                if (!activated)
                {
                    last_activation = Time.time;
                    activate_dropdown();
                }
                else
                {
                    last_activation = Time.time;
                    deactivate_dropdown();
                }
            }
        }
        Hovering = Time.time;
    }
    void OnTriggerExit(Collider exit)
    {
        if (!interactable_)
        {
            return;
        }
        if (exit.gameObject.layer == LayerMask.NameToLayer("Hovering"))
        {
            if (onExit != null)
            {
                onExit.Invoke();
            }
            Hovering = -1;
        }
        if (exit.gameObject.layer == LayerMask.NameToLayer("Activating"))
        {
            Hovering = -1;
        }
    }
    public void closeAndPick(int chosen)
    {
        activated = false;
        foreach (Transform t in this.transform)
        {
            if (t.gameObject.name != "Label")
            {
                Destroy(t.gameObject);
            }
        }
        onValueChanged.Invoke(chosen);
        label.text = options[chosen].label;
    }
}