using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour {

    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 200;
    public CircularQueue<Vector3> queue;


    void Start()
    {
        queue = new CircularQueue<Vector3>(lengthOfLineRenderer);
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0.0f), new GradientAlphaKey(1, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;
    }

    void Update()
    {

        Vector3 newPos = Input.mousePosition;
        newPos = Camera.main.ScreenToWorldPoint(new Vector3(newPos.x, newPos.y, 10));
        queue.Enqueue(newPos);


        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.SetPositions(queue.ToArray());
    }

}
