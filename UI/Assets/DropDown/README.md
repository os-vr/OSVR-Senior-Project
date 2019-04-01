DropDowns

To make a DropDown place The DropDownControl on a object with a collider set to trigger, and a textmeshpro component in its children

Drop Down Box refers to the actual box that the user clicks to open the drop down
Drop Down Item refers to all the items in the drop down list

You require a few peices that have the requirements listed below:
   BoundingBox: No requirements, this will be spawned and should bound you items
   Template: This must have a textmeshpro and a collider set to trigger as a prefab
   Extras: object that are spawned with the corresponding item in the dropdown
   
inspector details:
   Change In Height: difference in height between items in the drop down
   Hang Percentage: how much over the discrete cutoff a item can go and still be shown
   Drop Down Box Gradient: weather the dropdown box will use the Grad gradient
   Drop Down Item Gradient: weather the items will use the main gradient or thier individual gradients
   Grad: gradient for the Drop Down Box
   Button Grad: gradient for the Drop Down Items
   
   Options: This is the list of items that will be in your item list
   Label: the string for this item
   Button Grad: the gradient used for this button
   Pressed, Disabled, Hovered: internals that can be toggled to change state
   Extras: These items will be spawned at the item's position when in the right state.
           If normal, then Extras is placed. If pressed, then Extras pressed is placed.
           If hovered, then Extras Hovered is placed. If disabled, the extras disabled is placed
   There are 4 states: normal < pressed < hovered < diabled these correspond to the items that are placed
       each item suppercedes the ones on the left so the system is in the state furthest right that it can be.
   
