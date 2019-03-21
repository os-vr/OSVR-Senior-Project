using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    public LayerMask hovering;
    public LayerMask activating;
    private bool interactable_past = true;
    private bool Interactable = true;
    public bool interactable_
    {
        get
        {
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

    [Tooltip("The distance from base item and the first item of the dropdown")]
    public float firstOffset;

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

    [Tooltip("gradient for the box")]
    public Gradient boxGradient;

    [Tooltip("how long will the gradient transition take for the box")]
    public float BoxFade = 1.0f;

    [Tooltip("delay before holding a button is considered a second input")]
    public float delay = 0.1f;

    [Tooltip("Will the dropdown items use gradient or switch implementation")]
    public bool DropDownItemGradient = true;

    [Tooltip("gradient for the items")]
    public Gradient buttonGradient;

    [Tooltip("how long will the gradient transition take for the item")]
    public float ItemFade = 1.0f;

    public Color startColor;

    public Color endColor;

    [HideInInspector]
    public TextMesh label;

    public UnityEvent onDropClick;

    public UnityEvent onSelectClick;

    public UnityEvent onHover;

    public UnityEvent onEnter;

    public UnityEvent onExit;

    bool activated;

    [System.Serializable]
    public class onValueChange : UnityEvent<int> { };
    [SerializeField]
    public onValueChange onValueChanged = new onValueChange();

    [SerializeField]
    public Option[] options;

    float Hovering = -1;
    float hoverTime = 0;
    float slider = 0;
    GameObject[] scrollingOptions;
    float last_activation = -1;
    bool cancel = false;
    private NewButtonScript DropButton;
    void Awake()
    {
        DropButton = this.GetComponent<NewButtonScript>() as NewButtonScript;
        DropButton.onPress.AddListener(activate_dropdown);
        DropButton.setActivatinglayer(activating);
        DropButton.setHoveringlayer(hovering);
        maxOptionsShown = options.Length;
        this.label = this.GetComponentInChildren<TextMesh>();
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
        label = this.gameObject.GetComponentInChildren<TextMesh>();
        activated = false;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if (interactable != interactable_past)
        {
            interactable_ = interactable;
            interactable_past = interactable;
        }
        if (DropDownBoxGradient)
        {
            if (hoverTime > 0)
            {
                if (BoxFade == 0)
                {
                    hoverTime = 0;
                }
                else
                {
                    hoverTime -= Time.deltaTime * (1 / BoxFade);
                }
            }
            if (Hovering - Time.time > -0.08)
            {
                if (BoxFade == 0)
                {
                    hoverTime = 1;
                }
                else
                {
                    hoverTime += Time.deltaTime * 2 * (1 / BoxFade);
                }
            }
            if (hoverTime > 1)
            {
                hoverTime = 1;
            }
            this.GetComponent<Renderer>().material.color = boxGradient.Evaluate(hoverTime);
        }
    }
    */
    void deactivate_dropdown()
    {
        print("hammer" + (Time.time - last_activation)+"<"+delay);
        if (Time.time-last_activation < delay)
        {
            last_activation = Time.time;
            return;
        }
        last_activation = Time.time;
        activated = false;
        foreach (Transform t in this.transform)
        {
            if (t.gameObject.name != "Label")
            {
                Destroy(t.gameObject);
            }
        }
        DropButton.onPress.RemoveListener(deactivate_dropdown);
        DropButton.onPress.AddListener(activate_dropdown);
    }
    void activate_dropdown()
    {
        if (Time.time - last_activation < delay)
        {
            last_activation = Time.time;
            return;
        }
        last_activation = Time.time;
        DropButton.onPress.RemoveListener(activate_dropdown);
        DropButton.onPress.AddListener(deactivate_dropdown);
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
                GameObject inst = Instantiate(template, this.transform.position + this.transform.up * ((-i * changeInHeight) - firstOffset), this.transform.rotation);
                inst.transform.parent = this.transform;
                Text label = inst.AddComponent<Text>();
                label.text = op;
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
                GameObject DDinst = inst.gameObject.GetComponentInChildren<Collider>().gameObject;
                NewButtonScript DDI = DDinst.AddComponent<NewButtonScript>() as NewButtonScript;
                if (extra != null)
                {
                    extra.transform.parent = inst.transform;
                }
                DDI.gradient = options[i].buttonGrad;
                DDI.extras = options[i].extras;
                DDI.extrasHovered = options[i].extrasHovered;
                DDI.extrasPressed = options[i].extrasPressed;
                DDI.extrasDisabled = options[i].extrasDisabled;
                DDI.ItemFade = ItemFade;
                DDI.setActivatinglayer(activating);
                DDI.setHoveringlayer(hovering);
                DDI.extras = extra;
                int index = i;
                UnityAction UA = () => closeAndPick(index);
                DDI.onPress.AddListener(UA);
                TextMesh text = inst.GetComponentInChildren<TextMesh>();
                text.text = op;
                if (DropDownItemGradient)
                {
                    DDI.gradient = buttonGradient;
                }
                else
                {
                    DDI.gradient = options[i].buttonGrad;
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
                oldSlider.gameObject.SetActive(true);
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
    /*
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
        if (((1 << enter.gameObject.layer) & hovering) != 0)
        {
            if (onHover != null)
            {
                onHover.Invoke();
            }
        }
        if (((1 << enter.gameObject.layer) & activating) != 0)
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
        if (exit.gameObject.layer == hovering)
        {
            if (onExit != null)
            {
                onExit.Invoke();
            }
            Hovering = -1;
        }
        if (exit.gameObject.layer == activating)
        {
            Hovering = -1;
        }
    }
    */
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
        if (chosen != scrollingOptions.Length)
        {
            onValueChanged.Invoke(chosen);
            label.text = options[chosen].label;
        }
    }
    public void setHoveringlayer(LayerMask hovering)
    {
        DropButton.setHoveringlayer(hovering);
    }
    public void setActivatinglayer(LayerMask activating)
    {
        DropButton.setActivatinglayer(activating);
    }
}