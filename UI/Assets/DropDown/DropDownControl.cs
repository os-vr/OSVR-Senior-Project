using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace OSVR.UI
{
/// <summary>
/// This should be attached to a object that will serve as the base of the dropdown
/// The game object must also have a NewButtonScript to handle the button
/// </summary>
/// <remarks>
/// most of the functionality is ported to the NewButtonScript or to how the Game objects that
/// serve as templates are set up.
/// </remarks>
    public class DropDownControl : MonoBehaviour
    {
        /// <summary>
        /// This is an object that contains all the needed information about the dropdown buttons
        /// </summary>
        /// <remarks>
        /// This is designed to be used in a array that holds all the dropdowns. 
        /// The serialization also helps make all the items able to be set in the inspector
        /// </remarks>
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
            [SerializeField]
            public Color StartColor;
            [SerializeField]
            public Color EndColor;
            [SerializeField]
            public bool UseGradient;
            [SerializeField]
            public bool UseInstantTransition;
            public Option()
            {

            }
        }
        /// <summary>
        /// this is the bool used to determine if the item is interactable
        /// </summary>
        public bool interactable = true;
        /// <summary>
        /// This is the layer mask that will be used to determine if the buttons are being hovered on
        /// </summary>
        [Tooltip("This is the layer mask that will be used to determine if the buttons are being hovered on")]
        public LayerMask hovering;
        /// <summary>
        /// This is the layer mask that will be used to determine if the buttons are being activated
        /// </summary>
        [Tooltip("This is the layer mask that will be used to determine if the buttons are being activated")]
        public LayerMask activating;
        //The folowing is all legacy code that may be used at a later date to enforce the bool interactable onto all the buttons on the dropdown
        private bool interactable_past = true;
        private bool Interactable = true;
        /// <summary>
        /// Whether the dropdown is interactable, sets all items to off, but will not set all items to on if they were off before
        /// </summary>
        /// <value>
        /// The boolean that represents the state of the system
        /// </value>
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
                        op.GetComponentInChildren<ButtonScript>().setEnabled(value);
                    }
                }
            }
        }
        /// <summary>
        /// The sub item that will be created in the dropdown
        /// </summary>
        [Tooltip("The sub item that will be created in the dropdown")]
        public GameObject template;

        /// <summary>
        /// The difference in the heights of the item when drawn
        /// </summary>
        [Tooltip("The difference in the heights of the item when drawn")]
        public float changeInHeight;

        /// <summary>
        /// The distance from base item and the first item of the dropdown
        /// </summary>
        [Tooltip("The distance from base item and the first item of the dropdown")]
        public float firstOffset;

        /// <summary>
        /// The item that will surround all the items in the dropdown
        /// </summary>
        [Tooltip("The item that will surround all the items in the dropdown")]
        public GameObject boundingBox;

        /// <summary>
        /// The option that is selected if there is no input
        /// </summary>
        [Tooltip("The option that is selected if there is no input")]
        public string defaultString = "default";

        /// <summary>
        /// Where in the list the item will appear in the drop down. If it will not appear in the dropdown, set to -1
        /// </summary>
        [HideInInspector]
        [Tooltip("Where in the list the item will appear in the drop down. If it will not appear in the dropdown, set to -1")]
        public int defaultnumber = -1;

        /// <summary>
        /// How many options you can display
        /// </summary>
        [Tooltip("How many options you can display?")]
        public int maxOptionsShown = 5;

        /// <summary>
        /// Legacy code for implementing slider, may see the light of day pending slider support.
        /// Currently uses a normal Unity slider, but this would require a legitimate VR slider to be of use.
        /// </summary>
        [Tooltip("Old implementation of sliders for testing")]
        public Slider oldSlider;
        /// <summary>
        /// Affects when an item will be faded out in the slider implementation
        /// </summary>
        [Tooltip("Affects when an item will be faded out in the slider implementation")]
        public float hangpercentage = 0.5f;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("delay before holding a button is considered a second input")]
        public float delay = 0.1f;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("Will the dropdown items use gradient")]
        public bool UseGradient = false;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("Will the dropdown items use switch implementation")]
        public bool UseInstantTransition = false;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("gradient for the items")]
        public Gradient DropDownItemGradient;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("how long will the gradient transition take for the item")]
        public float ItemFade = 1.0f;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("The starting color if the color transition is instant")]
        public Color startColor;

        /// <summary>
        /// Will the dropdown box use gradient or switch implementation
        /// </summary>
        [Tooltip("The ending color if the color transition is instant")]
        public Color endColor;

        [HideInInspector]
        public TextMesh label;

        public UnityEvent onHover;

        public UnityEvent onEnter;

        public UnityEvent onExit;

        /// <summary>
        /// tells system if it should delay inputs to ensure there is no button spam based on what buttons were pressed
        /// </summary>
        bool activated;

        /// <summary>
        /// custom event to tell listeners what item was selected
        /// </summary>
        [System.Serializable]
        public class OnValueChange : UnityEvent<int> { };
        [SerializeField]
        public OnValueChange onValueChanged = new OnValueChange();

        /// <summary>
        /// lets users create buttons in the menu, contains information about making buttons on dropdown activation
        /// </summary>
        [SerializeField]
        public Option[] options;

        //legacy code, to be pruned
        float Hovering = -1;
        float hoverTime = 0;
        float slider = 0;
        //end of legacy
        /// <summary>
        /// Game objects made by the script for dropdown options
        /// </summary>
        GameObject[] scrollingOptions;
        /// <summary>
        /// ensures buttons can be held but not spammed by assuming any input that happens between the last input time
        /// plus the delay can be ignored, since the button is being held, and resets the value, so the button can be
        /// held as long as the user wants without having multiple inputs.
        /// </summary>
        float last_activation = -1;
        /// <summary>
        /// ensures buttons are not spammed by not taking input on the main button when a cancel has happened, 
        /// since the button to activate and deactive are the same.
        /// </summary>
        bool cancel = false;
        /// <summary>
        /// The button in the current game object that this script offloads the button functionality onto.
        /// </summary>
        private ButtonScript DropButton;
        /// <summary>
        /// set up of the object that happens before start
        /// </summary>
        void Awake()
        {
            //find and set button
            DropButton = this.GetComponent<ButtonScript>() as ButtonScript;
            if (DropButton == null)
            {
                DropButton = this.gameObject.AddComponent<ButtonScript>();
            }
            if (template == null)
            {
                Debug.Log("You need a template for your dropdown buttons, even if that is an empty object with text");
            }
            if (delay <= 0.03)
            {
                delay = 0.03f;
                Debug.Log("Putting the delay to low may cuase multiple inputs for one action. Also, it may be a seizure hazard");
            }
            //will produce errors if no button is set change to give readible error
            DropButton.onPress.AddListener(activate_dropdown);
            //set the layers
            DropButton.setActivatinglayer(activating);
            DropButton.setHoveringlayer(hovering);
            maxOptionsShown = options.Length;
            //handle the text
            this.label = this.GetComponentInChildren<TextMesh>();
            label.text = defaultString;
            //fixing testing problems with slider
            if (oldSlider != null)
            {
                oldSlider.gameObject.active = false;
            }
            //handles starting default string
            //negative means start at default string
            if (defaultnumber < 0)
            {
                label.text = defaultString;
            }
            else
            {
                //if there is no default string pick the item at the number
                if (defaultString.Length == 0)
                {
                    label.text = options[defaultnumber].label;
                }
                else
                {
                    //resize to include the default
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
        /// <summary>
        /// Use this for initialization after awake
        /// </summary>
        void Start()
        {
            label = this.gameObject.GetComponentInChildren<TextMesh>();
            activated = false;
        }
        // Update is called once per frame, only handles interactable, which is used for real time testing
        /// <summary>
        /// Update is called once per frame, only handles interactable, which is used for real time testing
        /// </summary>
        void Update()
        {
            if (interactable != interactable_past)
            {
                interactable_ = interactable;
                interactable_past = interactable;
            }
        }
        /*
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
        /// <summary>
        /// deconstructs the dropdown
        /// </summary>
        void deactivate_dropdown()
        {
            //handles spammed commands
            if (Time.time - last_activation < delay)
            {
                last_activation = Time.time;
                return;
            }
            //reset clock
            last_activation = Time.time;
            activated = false;
            //deconstruct
            foreach (Transform t in this.transform)
            {
                if (t.gameObject.name != "Label")
                {
                    Destroy(t.gameObject);
                }
            }
            //handles the listeners
            DropButton.onPress.RemoveListener(deactivate_dropdown);
            DropButton.onPress.AddListener(activate_dropdown);
        }
        /// <summary>
        /// constructs the dropdown
        /// </summary>
        void activate_dropdown()
        {
            //handle spammed commands
            if (Time.time - last_activation < delay)
            {
                last_activation = Time.time;
                return;
            }
            if (activated == false && interactable_)
            {
                last_activation = Time.time;
                //handle the listeners
                /*It is assumed every interaction with the dropdown is a single button interaction, for multiple actions the
            onPress listener must have another function handle input and only use deactivate_dropdown when the dropdown
            is closed.*/
                DropButton.onPress.RemoveListener(activate_dropdown);
                DropButton.onPress.AddListener(deactivate_dropdown);
                activated = true;
                //makes boundry
                if (boundingBox != null)
                {
                    GameObject BoundingBox = Instantiate(boundingBox, this.transform.position, this.transform.rotation);
                    BoundingBox.transform.SetParent(this.transform);
                }
                bool scrolling = false;
                scrollingOptions = new GameObject[options.Length];
                //for scrolling support, not currently supported, currently maxOptionsShown < options.Length is always false
                if (maxOptionsShown < options.Length)
                {
                    scrolling = true;
                }
                //considering changing i to position or index, somehting more descriptive.
                for (int i = 0; i < options.Length; i++)
                {
                    //makes each item in options
                    string op = options[i].label;
                    //make template
                    GameObject inst = Instantiate(template, this.transform.position + this.transform.up * ((-i * changeInHeight) - firstOffset), this.transform.rotation);
                    inst.transform.parent = this.transform;
                    Text label = inst.AddComponent<Text>();
                    label.text = op;
                    GameObject extra = null;
                    Option opt = options[i];
                    //spawns extras where appropriate
                    if (opt.Disabled)
                    {
                        if (opt.extrasDisabled != null)
                        {
                            extra = Instantiate(opt.extrasDisabled, this.transform.position + this.transform.up * -i * changeInHeight, this.transform.rotation);
                        }
                    }
                    else
                    {
                        if (opt.Hovered)
                        {
                            if (opt.extrasHovered != null)
                            {
                                extra = Instantiate(opt.extrasHovered, this.transform.position + this.transform.up * -i * changeInHeight, this.transform.rotation);
                            }
                        }
                        else
                        {
                            if (opt.Pressed)
                            {
                                if (opt.extrasPressed != null)
                                {
                                    extra = Instantiate(opt.extrasPressed, this.transform.position + this.transform.up * -i * changeInHeight, this.transform.rotation);
                                }

                            }
                            else
                            {
                                if (opt.extras != null)
                                {
                                    extra = Instantiate(opt.extras, this.transform.position + this.transform.up * -i * changeInHeight, this.transform.rotation);
                                }
                            }
                        }
                    }
                    //gets the created gameobject with the collider
                    GameObject DDinst = inst.gameObject.GetComponentInChildren<Collider>().gameObject;
                    //DDi stands for Drop Down Item a deprecated script but a useful name. It is actually a new button script
                    ButtonScript DDI = DDinst.AddComponent<ButtonScript>() as ButtonScript;
                    //may be incorrect
                    if (extra != null)
                    {
                        extra.transform.parent = inst.transform;
                    }
                    //set values from options
                    DDI.extras = options[i].extras;
                    DDI.extrasHovered = options[i].extrasHovered;
                    DDI.extrasPressed = options[i].extrasPressed;
                    DDI.extrasDisabled = options[i].extrasDisabled;
                    DDI.ItemFade = ItemFade;
                    DDI.setActivatinglayer(activating);
                    DDI.setHoveringlayer(hovering);
                    DDI.extras = extra;
                    int index = i;
                    //create custom listener that has the number of the item picked preset
                    UnityAction UA = () => closeAndPick(index);
                    DDI.onPress.AddListener(UA);
                    //set text
                    TextMesh text = inst.GetComponentInChildren<TextMesh>();
                    text.text = op;
                    //determines which gradient the item will use
                    DDI.useGradient = this.UseGradient;
                    DDI.useInstantTransition = this.UseInstantTransition;
                    if (UseInstantTransition)
                    {
                        DDI.useInstantTransition = true;
                        DDI.startColor = this.startColor;
                        DDI.endColor = this.endColor;
                    }
                    else
                    {
                        if (UseGradient)
                        {
                            DDI.gradient = this.DropDownItemGradient;
                            DDI.useGradient = true;
                        }
                        else
                        {
                            DDI.startColor = options[i].StartColor;
                            DDI.endColor = options[i].EndColor;
                            DDI.gradient = options[i].buttonGrad;
                            DDI.useGradient = options[i].UseGradient;
                            DDI.useInstantTransition = options[i].UseInstantTransition;
                        }
                    }
                    if (i > maxOptionsShown - 1)
                    {
                        inst.SetActive(false);
                    }
                    //set the array
                    scrollingOptions[i] = inst;
                }
                //not used
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
        //slider logic to be used when sliders are ready
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
        //this has all been passed on to the new button script
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
        /// <summary>
        /// This will handle the selection process and the event handling, calls the deconstruction.
        /// </summary>
        /// <param name="chosen">The integer of the index of the item in the options array that was picked</param>
        //The team doesn't know why this works, it does, but it doesn't call deactivate dropdown. The team will look into this further.
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
                //invokes the reporting event
                onValueChanged.Invoke(chosen);
                label.text = options[chosen].label;
            }
        }
        /// <summary>
        /// setter for the hovering layer
        /// </summary>
        /// <param name="hovering">The layer mask we are using for hovering</param>
        public void setHoveringlayer(LayerMask hovering)
        {
            DropButton.setHoveringlayer(hovering);
        }
        /// <summary>
        /// setter for the activating layer
        /// </summary>
        /// <param name="activating">The layer mask we are using for activating</param>
        public void setActivatinglayer(LayerMask activating)
        {
            DropButton.setActivatinglayer(activating);
        }
    }
}
