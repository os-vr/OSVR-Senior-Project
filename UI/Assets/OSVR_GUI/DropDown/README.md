DropDowns

To make a DropDown place The DropDownControl on a object, a newbuttonscript on the object with a collider set to trigger, and a textmeshpro component in its children

Drop Down Box refers to the actual box that the user clicks to open the drop down 
Drop Down Item refers to all the items in the drop down list

You require a few peices that have the requirements listed below: 
BoundingBox: No requirements, this will be spawned and should bound you items 
Template: This must have a textmeshpro, a newbuttonscript, and a collider set to trigger as a prefab 
Extras: object that are spawned with the corresponding item in the dropdown

inspector details: 
defaultString: The default string for the dropdown when nothing is selected. 
defaultnumber: The index at which that defualt string appears in the results of the dropdown. 
Use -1 if it will not appear in the options shown. 
maxOptionsShown: number of options shown until a slider is used, for now, 
this is set to the number of options total since sliders are not fully implemented.

Change In Height: difference in height between items in the drop down 
Hang Percentage: how much over the discrete cutoff a item can go and still be shown 
Drop Down Gradient: Weather the dropdown box will use the Grad gradient 
Drop Down Item Gradient: Weather the dropdown items will use the gradient or instant color switch. 
Grad: gradient for the Drop Down 
Box gradient: The gradient for color changing based on hovering. 
useGradient: If we use the given gradient. 
startColor: The color we start with if we instantly change the color. 
endColor: The color we end with if we instantly change color. 
boxFade: How long the color transition takes when hovered on the dropdownbox. 
itemFade: How long the color transition takes when hovered on the dropdownitems. 
delay: how long no input must be sent for after which input will be considered valid. 
onEnterUseForHover: Determines if we use onEnter events for hovering. 
onEnterUseForActivation: Determines if we use onEnter events for activating. 
onStayUseForHover: Determines if we use onStay events for hovering. 
onStayUseForActivation: Determines if we use onStay events for activating. 
onExitUseForHover: Determines if we use onExit events for hovering. 
onExitUseForActivation: Determines if we use onExit events for activating. 
detectOnEnter: Determines if we use any event on enter. 
detectOnHover: Determines if we use any events on hover. 
detectOnExit: Determines if we use any events on exit.

Options: This is the list of items that will be in your item 
list Label: The string for this item 
Button Grad: The gradient used for this button 
Pressed, Disabled, Hovered: internals that can be toggled to change state 
extras: gameobjects that are rendered when the botton is in no other state. 
extrasHovered: gameobjects that are rendered when the botton is in the hovered state. 
extrasPressed: gameobjects that are rendered when the botton is in the pressed state. 
extrasDisabled: gameobjects that are rendered when the botton is in the disabled state. 
Extras: These items will be spawned at the item's position when in the right state. 
If normal, then Extras is placed. If pressed, then Extras pressed is placed. 
If hovered, then Extras Hovered is placed. If disabled, the extras disabled is placed 
There are 4 states: normal < pressed < hovered < diabled these correspond to the items 
that are placed each item suppercedes the ones on the left so the system is in the state furthest right that it can be.

Unity Events:

onValueChange(int): The index of the item that was selected. 
onDropClick: Called when the drop down box is clicked. 
onSelectClick: Called when an item is selected. 
onHover: Called when the dropdown box has an collider stay in it; 
onEnter: Called when the dropdown box has an collider enter in it; 
onExit: Called when the dropdown box has an collider exit in it;
