NewButtonScript

To make a NewButtonScript place The NewButtonScript on a object with a collider set to trigger

inspector details: 
hoverLayer/activationLayer: The layer mask used to determine if a colliding object is in the layers that hovers or activates. 
extra: The game object that is rendered in addition to the button, based on the button's state. 
triggered: Determines if the button has been pressed. gradient: 
The gradient for color changing based on hovering. 
useGradient: If we use the given gradient. 
startColor: The color we start with if we instantly change the color. 
endColor: The color we end with if we instantly change color. 
itemFade: How long the color transition takes when hovered. 
delay: how long no input must be sent for after which input will be considered valid. 
onEnterUseForHover: Determines if we use onEnter events for hovering. 
onEnterUseForActivation: Determines if we use onEnter events for activating. 
onStayUseForHover: Determines if we use onStay events for hovering. 
onStayUseForActivation: Determines if we use onStay events for activating. 
onExitUseForHover: Determines if we use onExit events for hovering. 
onExitUseForActivation: Determines if we use onExit events for activating. 
detectOnEnter: Determines if we use any event on enter. 
detectOnHover: Determines if we use any events on hover. 
\detectOnExit: Determines if we use any events on exit. 
extras: gameobjects that are rendered when the botton is in no other state. 
extrasHovered: gameobjects that are rendered when the botton is in the hovered state. 
extrasPressed: gameobjects that are rendered when the botton is in the pressed state. 
extrasDisabled: gameobjects that are rendered when the botton is in the disabled state.

Pressed, Disabled, Hovered: internals that can be toggled to change state 
These items will be spawned at the item's position when in the right state.

If normal, then Extras is placed. If pressed, then Extras pressed is placed. 
If hovered, then Extras Hovered is placed. 
If disabled, the extras disabled is placed 
There are 4 states: normal < pressed < hovered < diabled these correspond to the items 
that are placed each item suppercedes the ones on the left so the system is in the state furthest right that it can be.

Unity Events:

onHover: Called when the dropdown box has an collider stay in it; 
onEnter: Called when the dropdown box has an collider enter in it; 
onExit: Called when the dropdown box has an collider exit in it;
