using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewingNormalizer : Normalizer {

    Camera m;

    public ViewingNormalizer()
    {
        m = Camera.main;
    }

    public GTransform Normalize(GTransform g)
    {
        Vector3 screenPoint = m.WorldToScreenPoint(g.position);
        Vector3 finalPosition = new Vector3(screenPoint.x / Screen.width, screenPoint.y / Screen.height, Vector3.Distance(m.transform.position, g.position));

        //TODO: get angle normalization.
        return new GTransform(finalPosition, Quaternion.identity);
    }
}
