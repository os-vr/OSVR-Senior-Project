using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class ContinuousGestureMonitor : MonoBehaviour
{
    Dictionary<string, Gesture> gestureMap;
    public IController controller;
    private Normalizer n;
    public GameObject obj;

    // Use this for initialization
    void Start()
    {
        n = new ViewingNormalizer();

        gestureMap = new Dictionary<string, Gesture>();

        // add gestures here for now, looking to add support for Inspector-based gesture adding in the future
        //AddGesture("Rightward Swipe", new SidewaysSwipe(true)); // too much overlap with LSwipe
        AddGesture("Leftward Swipe", new SidewaysSwipe(false));
        AddGesture("L Swipe", new LSwipe());
        AddGesture("UpsideDown L Swipe", new UpsideDownL());

        Cursor.visible = false;
    }
    
    void Update()
    {
        Vector3 newPos = Input.mousePosition;
        newPos = Camera.main.ScreenToWorldPoint(new Vector3(newPos.x, newPos.y, 1.0f));
        GTransform nextDataPoint = new GTransform(newPos, Quaternion.identity);
        obj.transform.position = newPos;
        //GTransform nextDataPoint = controller.QueryGTransform();
        //pathRenderer.SetPath(dataQueue);

        foreach (Gesture g in gestureMap.Values)
        {
            g.CheckStages(nextDataPoint);
        }


    }

    void AddGesture(string name, Gesture g)
    {
        gestureMap.Add(name, g);
    }
    void SetGestureEnabled(string name, bool enabled)
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
