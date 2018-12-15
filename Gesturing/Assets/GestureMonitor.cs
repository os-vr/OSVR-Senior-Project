using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GestureMonitor : MonoBehaviour {
    PathRenderer pathRenderer;
    Dictionary<string, Gesture> gestureMap;
    Dictionary<string, Normalizer> normalizationMap;
    CircularQueue dataQueue;
    public Normalizer DefaultNormalizer;
    public TextMeshPro t;

    // Use this for initialization
    void Start() {
        DefaultNormalizer = new ViewingNormalizer();
        pathRenderer = gameObject.AddComponent<PathRenderer>();

        gestureMap = new Dictionary<string, Gesture>();
        dataQueue = new CircularQueue(100);

        Gesture g = new OverheadSwipeGesture(new Vector3(0, 1, .3f), new Vector3(1, 1, .3f), .5f, new ViewingNormalizer());
        UnityEvent e = new UnityEvent();
        e.AddListener(delegate { t.text = "Overhead Gesture Detected!"; });
        g.AddEvent(e);
        AddGesture("overhead", g);
        drawStages();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 newPos = Input.mousePosition;
        newPos = Camera.main.ScreenToWorldPoint(new Vector3(newPos.x, newPos.y, .25f));
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

    void AddGesture(string name, Gesture g, Normalizer n)
    {
        AddGesture(name, g);
        normalizationMap.Add(name, n);
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
