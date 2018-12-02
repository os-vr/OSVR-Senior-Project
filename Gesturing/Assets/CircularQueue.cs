using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularQueue : IEnumerable<GTransform> {

    private int maxSize;
    private LinkedList<GTransform> data;

    public CircularQueue(int maxSize){
        this.data = new LinkedList<GTransform>();
        this.maxSize = maxSize;
    }

    public void Enqueue(GTransform t)
    {
        data.AddFirst(t);
        if(data.Count > maxSize)
        {
            data.RemoveLast();
        }
    }

    public void Dequeue()
    {
        data.RemoveLast();
    }

    public void Clear()
    {
        data.Clear();
    }

    public IEnumerator<GTransform> GetEnumerator()
    {
        foreach(GTransform t in data)
        {
            yield return t;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Vector3[] ToArray()
    {
        Vector3[] ret = new Vector3[maxSize];
        int counter = 0;
        foreach (GTransform g in data)
        {
            ret[counter++] = g.position;
        }
        return ret;
    }

}
