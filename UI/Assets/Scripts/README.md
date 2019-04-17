## ScaleToUser 

To make this script work, put this script on the item you want to scale and set the following:

**userCamera**: The camera that we use to do calculations of distance from the item. 

**pivotPoint**: The object that will be used to find disance between user and item for scale. 

**isCenter**: Determines if this item is the item we use for determining distance for scaling. 

**ScaleDist**: Distance at which we expect the item to be the original scale, any item further away than this will scale at the far scale rate, any item closer will scale at the nearScale rate. 

**farScale/nearScale**: Determines the rate of linear change in size based on distance. 
a value of 1 will make the item stay the same size, more than 1 will make the item grow, 
a value of less will make the items shrink as you move away but at a lower speed.



## RotateToUser 

To make this script work, put this script on the item you want to face the user and set the following:

**userCamera**: The camera that we use to do calculations of rotation from the item. 

**pivotPoint**: The object that will be used to find rotation between user and item. 

**distanceFromPivotPoint**: This will make sure the item is orbiting at the correct distance.

note: If your object moves inaddition to rotating to user, make the pivot point the object that moves, 
and make sure the rotating object is not a child of that object.



## DisappearAtDistance 

To make this script work, put this script on the item you want to disappear 
when outside of a certian range of distances from the user and set the following:

**userCamera**: The camera that we use to do calculations of distance to item. 

**pivotPoint**: The object that will be used to find distance between user and item. 

**isCenter**: Determines if this item is the item we use for determining distance for distance finding. 

**disappearAtCloseDistance**: The item will disappear if it is closer than that distance. 

**disapearWhenFarAway**: Determines if the item will disappear at beyond a certian distance. 

**disapearAtFarDistance**: The item will disappear if it is further than this distance if disapearWhenFarAway is true

note: This manually turns off the renders, so if any other scripts modify visibility this may turn off renderes 
and turn them on depending on what would be needed to fit the constraints. This could lead to renderers you set to off being turned on.
