using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureMonitor : MonoBehaviour {
    PathRenderer pathRenderer;
    Dictionary<string, Gesture> gestureMap;
    CircularQueue dataQueue;

	// Use this for initialization
	void Start () {

        pathRenderer = gameObject.AddComponent<PathRenderer>();

        gestureMap = new Dictionary<string, Gesture>();
        dataQueue = new CircularQueue(100);

        Gesture gesture = new Gesture(new List<Stage> { new Stage(), });
        gestureMap["Default"] = gesture;

        drawStages();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = Input.mousePosition;
        newPos = Camera.main.ScreenToWorldPoint(new Vector3(newPos.x, newPos.y, 10));
        GTransform nextDataPoint = new GTransform(newPos, Quaternion.identity);
        dataQueue.Enqueue(nextDataPoint);

        pathRenderer.SetPath(dataQueue);



        foreach(Gesture g in gestureMap.Values)
        {
            g.CheckStages(nextDataPoint);
        }

    }

    void AddGesture(string name, Gesture g)
    {
        gestureMap.Add(name, g);
    }

    void setGestureEnabled(string name, bool enabled)
    {
        if (gestureMap.ContainsKey(name))
        {
            gestureMap[name].isEnabled = enabled;
        }
    }



    void drawStages()
    {
     
    }

}
