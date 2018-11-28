using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularQueue<T> : IEnumerable<T> {

    private int maxSize;
    private LinkedList<T> data;

    public CircularQueue(int maxSize){
        this.data = new LinkedList<T>();
        this.maxSize = maxSize;
    }

    public void Enqueue(T t)
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

    public IEnumerator<T> GetEnumerator()
    {
        foreach(T t in data)
        {
            yield return t;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T[] ToArray()
    {
        T[] ret = new T[maxSize];
        data.CopyTo(ret, 0);
        return ret;
    }

}
