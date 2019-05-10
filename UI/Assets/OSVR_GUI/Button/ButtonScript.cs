using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace OSVR.UI
{
/// <summary>
/// Handles a basic button. This is used by other functions that use buttons
/// </summary>
/// <remarks>
/// Keyboard, and dropdown heavily rely on buttons
/// </remarks>
    public class ButtonScript : MonoBehaviour
    {
        /// <summary>
        /// The extra gameobject that is selected to be shown and exists in the gameworld
        /// </summary>
        [Tooltip("The extra gameobject that is selected to be shown and exists in the gameworld")]
        GameObject extra;        /// <summary>
        /// If the button has been pressed
        /// </summary>
        [Tooltip("If the button has been pressed")]
        bool triggered;
        /// <summary>
        /// used for hoving color changing
        /// </summary>
        float hovering = -1;
        /// <summary>
        /// The gradient for color changing
        /// </summary>
        [Tooltip("The gradient for color changing")]
        public Gradient gradient;
        /// <summary>
        /// Whether we use the gradient
        /// </summary>
        [Tooltip("Whether we use the gradient or instant transition")]
        public bool useGradient;
        /// <summary>
        /// The start color in instant transitions
        /// </summary>
        [Tooltip("The start color in instant transitions")]
        public Color startColor;
        /// <summary>
        /// The end color in instant transitions
        /// </summary>
        [Tooltip("The end color in instant transitions")]
        public Color endColor;
        /// <summary>
        /// Whether we use the instant transition
        /// </summary>
        [Tooltip("Whether we use the instant transition")]
        public bool useInstantTransition;
        /// <summary>
        /// The time it takes to get the hovered item to do a fade transition
        /// set to 0 for instant transitions
        /// </summary>
        [Tooltip("The time it takes to get the hovered item to do a fade transition")]
        public float ItemFade = 1.0f;
        /// <summary>
        /// The delay after which inputs are considered legitmate to reduce input spamming
        /// must be negative float in seconds
        /// </summary>
        [Tooltip("The delay after which inputs are considered legitmate to reduce input spamming")]
        public float delay = 0.08f;
        /// <summary>
        /// Used for getting the gradient color change
        /// </summary>
        float hoverTime = 0;

        /// <summary>
        /// If we use onEnter for the hovering layer
        /// </summary>
        [Tooltip("If we use onEnter for the hovering layer")]
        public bool onEnterUseForHover = true;
        /// <summary>
        /// If we use onEnter for the hovering layer
        /// </summary>
        [Tooltip("If we use onEnter for the activating layer")]
        public bool onEnterUseForActivation = true;

        public UnityEvent onEnter = new UnityEvent();

        /// <summary>
        /// If we use onStay for the hovering layer
        /// </summary>
        [Tooltip("If we use onStay for the hovering layer")]
        public bool onStayUseForHover = true;
        /// <summary>
        /// If we use onStay for the activating layer
        /// </summary>
        [Tooltip("If we use onStay for the activating layer")]
        public bool onStayUseForActivation = true;
        public UnityEvent onHover = new UnityEvent();

        /// <summary>
        /// If we use onExit for the hovering layer
        /// </summary>
        [Tooltip("If we use onExit for the hovering layer")]
        public bool onExitUseForHover = true;
        /// <summary>
        /// If we use onExit for the activating layer
        /// </summary>
        [Tooltip("If we use onExit for the activating layer")]
        public bool onExitUseForActivation = true;
        public UnityEvent onExit = new UnityEvent();

        public UnityEvent onPress = new UnityEvent();

        /// <summary>
        /// determines if we detect onEnter events
        /// </summary>
        [Tooltip("determines if we detect onEnter events")]
        public bool detectOnEnter = true;
        /// <summary>
        /// determines if we detect onHover events
        /// </summary>
        [Tooltip("determines if we detect onHover events")]
        public bool detectOnHover = true;
        /// <summary>
        /// determines if we detect onExit events
        /// </summary>
        [Tooltip("determines if we detect onExit events")]
        public bool detectOnExit = true;
        /// <summary>
        /// The layer that object are in to be considered hovering
        /// </summary>
        [Tooltip("The layer that object are in to be considered Hovering")]
        public LayerMask hoverLayer;
        /// <summary>
        /// The layer that object are in to be considered activating
        /// </summary>
        [Tooltip("The layer that object are in to be considered activating")]
        public LayerMask activationLayer;

        /// <summary>
        /// Determines if the button is being hovered
        /// </summary>
        [Tooltip("Determines if the button is being hovered")]
        public bool hovered;
        /// <summary>
        /// Determines if the button has been pressed
        /// </summary>
        [Tooltip("Determines if the button has been pressed")]
        public bool pressed;
        /// <summary>
        /// Determines if the button has been disabled
        /// </summary>
        [Tooltip("Determines if the button has been disabled")]
        public bool disabled;
        /// <summary>
        /// The normal item that are added to the button when it is not in any other state
        /// </summary>
        [Tooltip("The normal item that are added to the button when it is not in any other state")]
        public GameObject extras;
        /// <summary>
        /// The normal item that are added to the button when it is hovered
        /// </summary>
        [Tooltip("The normal item that are added to the button when it is hovered")]
        public GameObject extrasHovered;
        /// <summary>
        /// The normal item that are added to the button when it is pressed
        /// </summary>
        [Tooltip("The normal item that are added to the button when it is pressed")]
        public GameObject extrasPressed;
        /// <summary>
        /// The normal item that are added to the button when it is disabled
        /// </summary>
        [Tooltip("The normal item that are added to the button when it is disabled")]
        public GameObject extrasDisabled;
        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Start()
        {
            Collider col = this.GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                Debug.Log("Set your collider isTrigger to true");
                col.isTrigger = true;
            }
            else if(col == null)
            {
                Debug.Log("You need a collider");
                col = this.gameObject.AddComponent<Collider>();
                col.isTrigger = true;
            }
            triggered = false;
            if (startColor == null)
            {
                startColor = this.GetComponent<Renderer>().material.color;
            }
            if (extras != null)
            {
                extra = Instantiate(extras);
            }
            if (ItemFade <= 0)
            {
                ItemFade = 0.001f; 
            }
            if (useGradient && useInstantTransition)
            {
                Debug.LogError("You can only use one kind of transition, defaulting to Instant transition");
                useGradient = false;
            }
            
        }
        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            // are we hovering?
            hovered = ((Time.time - hovering) < delay);
            //fade out
            if (hoverTime > 0)
            {
                if (useInstantTransition)
                {
                    hoverTime = 1;
                }
                else
                {
                    hoverTime -= Time.deltaTime * (1 / ItemFade);
                }
            }
            //fade in
            if ((Time.time-hovering) < delay)
            {
                if (useInstantTransition)
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
                //handle instant color changing
                if (useInstantTransition)
                {
                    hoverTime = 0;
                }
            }
            //lower bound check
            if (hoverTime < 0)
            {
                hoverTime = 0;
            }
            //upper bound check
            if (hoverTime > 1)
            {
                hoverTime = 1;
            }
            //update extras
            nextStep();
            //disable
            if (disabled)
            {
                return;
            }
            //gradient handling
            if (useInstantTransition)
            {
                if (hoverTime == 1)
                {
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
            //actual color change
            if (useGradient)
            {
                this.GetComponent<Renderer>().material.color = gradient.Evaluate(hoverTime);
            }
        }
        /// <summary>
        /// This will handle trigger enter events for button functionality
        /// </summary>
        /// <param name="chosen">The collider that interacted with this object's collider</param>
        private void OnTriggerEnter(Collider other)
        {
            if (detectOnEnter && !disabled)
            {

                //LayerMask value is 2 to the power of its layer index
                if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value)
                {
                    //Debug.Log("Act Layer Enter");
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
                    //Debug.Log("Hover Layer Enter");
                    if (onEnter != null)
                    {
                        onEnter.Invoke();
                    }
                }

                else if (onEnterUseForActivation && onEnterUseForHover)
                {
                    //Debug.Log("Both Layer Enter");
                    pressed = true;
                    if (onEnter != null)
                    {
                        onEnter.Invoke();
                    }
                }
            }
        }
        /// <summary>
        /// This will handle trigger stay events for button functionality
        /// </summary>
        /// <param name="other">The collider that interacted with this object's collider</param>
        void OnTriggerStay(Collider other)
        {
            if (detectOnHover && !disabled)
            {

                hovered = true;

                if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value)
                {
                    //Debug.Log("Act Layer Stay");
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
                    //Debug.Log("Hover Layer Stay");
                    if (onHover != null)
                    {
                        onHover.Invoke();
                    }
                    hovering = Time.time;
                }
                else if (onStayUseForActivation && onStayUseForHover)
                {
                    //Debug.Log("Both Layer Enter");
                    pressed = true;
                    if (onHover != null)
                    {
                        onHover.Invoke();
                    }
                    hovering = Time.time;
                }
            }

        }
        /// <summary>
        /// This will handle trigger exit events for button functionality
        /// </summary>
        /// <param name="other">The collider that interacted with this object's collider</param>
        void OnTriggerExit(Collider other)
        {
            if (detectOnExit && !disabled)
            {
                if (Mathf.Pow(2, other.gameObject.layer) == activationLayer.value && onExitUseForActivation)
                {
                    //Debug.Log("Act Layer Exit");
                    pressed = true;
                    if (onExit != null)
                    {
                        onExit.Invoke();
                    }
                    hovering = -1;
                }

                else if (Mathf.Pow(2, other.gameObject.layer) == hoverLayer.value && onExitUseForHover)
                {
                    //Debug.Log("Hover Layer Exit");
                    if (onExit != null)
                    {
                        onExit.Invoke();
                    }
                    hovering = -1;
                }
                else if (onExitUseForActivation && onExitUseForHover)
                {
                    //Debug.Log("Both Layer Enter");
                    pressed = true;
                    if (onExit != null)
                    {
                        onExit.Invoke();
                    }
                    hovering = -1;
                }
            }
        }
        /// <summary>
        /// Enables the object
        /// </summary>
        /// <param name="value">boolean of enabled or not</param>
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
        /// <summary>
        /// handles disabling the button
        /// </summary>
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
        /// <summary>
        /// handles enabling the button
        /// </summary>
        /// <remarks>
        /// This is a function that calls another function. This only exists to have a consistent naming style.
        /// This may change in later updates.
        /// </remarks>
        void onEnable()
        {
            nextStep();
        }
        /// <summary>
        /// nextStep will update the extra game objects
        /// </summary>
        void nextStep()
        {
            if (disabled)
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
            else
            {
                Destroy(extra);
                if (hovered)
                {
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
        }
        /// <summary>
        /// Set whether the item is hovered
        /// </summary>
        /// <param name="state">whether the item is hovered</param>
        public void setHovered(bool state)
        {
            hovered = state;
            nextStep();
        }
        /// <summary>
        /// Set whether the item is pressed
        /// </summary>
        /// <param name="state">whether the item is pressed</param>
        public void setPressed(bool state)
        {
            pressed = state;
            nextStep();
        }
        /// <summary>
        /// set whether the item is disabled
        /// </summary>
        /// <param name="state">whether the item is disabled</param>
        public void setDisabled(bool state)
        {
            disabled = state;
            nextStep();
        }
        /// <summary>
        /// Set the hovering layer for the button
        /// </summary>
        /// <param name="hovering">The layermask that is used for hovering</param>
        public void setHoveringlayer(LayerMask hovering)
        {
            this.hoverLayer = hovering;
        }
        /// <summary>
        /// Set the activating layer for the button
        /// </summary>
        /// <param name="activating">The layermask that is used for activating</param>
        public void setActivatinglayer(LayerMask activating)
        {
            this.activationLayer = activating;
        }
    }
}
