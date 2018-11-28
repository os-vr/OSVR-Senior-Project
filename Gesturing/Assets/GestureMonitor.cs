using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureMonitor : MonoBehaviour {
    public PathRenderer renderer;
    Dictionary<string, Gesture> gestureMap;
    CircularQueue<GTransform> dataQueue;

	// Use this for initialization
	void Start () {
        gestureMap = new Dictionary<string, Gesture>();
        dataQueue = new CircularQueue<GTransform>(100);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AddGesture(string name, Gesture g)
    {
        gestureMap.Add(name, g);
    }

    void setGestureEnabled(string name, bool enabled)
    {
        if (gestureMap.ContainsKey(name))
        {
            gestureMap[name].SetIsEnabled(enabled);
        }
    }
}
