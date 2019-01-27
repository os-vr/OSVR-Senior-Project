# Open Source VR Project

## Gesturing Framework
This asset allows you to create detectable, customizable gestures to place into your Unity scenes. There are a few main components to learn before you can start working with gestures.

### Gesture Monitor
GesutreMonitor.cs is the main class that monitors Gestures. Attach an implementation of this (DiscreteGestureMonitor.cs, ContinuousGestureMonitor.cs or build your own) to an object in a script to detect gestures.

Gesture Monitors are the only class type that extend MonoBehaviour. All other classes in this framework are standalone C# classes or interfaces. 

#### Discrete Gesture Monitoring
This DiscreteGestureMonitor class checks for gesture occurnace after a specified amount of input (e.g., hold a button to make a gesture, then release to analyze).

#### Continuous Gesture Monitoring
This ContinuousGestureMonitoring class continuously checks for gestures to occur. Data is passed one point at a time through the Gestures. 

#### Alternate Gesture Monitoring
It is possible to subclass the GestureMonitor.cs class in order to implement other ways of feeding data to Gestures.

### Controller
This Interface generates the Gesture Data that the monitor passes into each of the active gestures. Currently, there is a TouchController for Oculus Rift, but extending this to work for any hardware is as simple as implementing the [function] to generate a GTransform

#### GTransform
This is the data object for gesture processing. It consists of a Vector3 position field, and Quaternion rotation field. 

### Gesture
The Gesture class consists of a few main components: Checks and Normalizers. Both of these objects are interfaces, so you can write your own classes to exhibit the desired functionality you would like in a gesture.

#### Checks
Checks are the subsections of a gesture that need to be satisfied for a gesture to be complete. For example, RadiusCheck.cs checks to see if the given point exists within a given radius. PrismCheck.cs checks to see if a point lies within a given prism-shaped boundary, etc. 

#### Normalizers
These functions take the data generated from the Controller Interface and convert them into a more usable form for the Gestures. For example, the FittedNormalizer.cs normalizes all points received to a bounding box of (0,0,0) to (1,1,1). The ViewingNormalizer converts all position data to the coordinates relative to a Camera object from (0,0) to (1,1) in x and y, and distance from screen in the z component. You can implement your own normalizers if you need more specific functionality.

Good normalization algorithms are very important for consistent gesture functionality. 

## UI Framework

## Future Updates
### Gesturing
More continuous monitoring support
Customized Integration with the Unity Inspector
### UI Framework
