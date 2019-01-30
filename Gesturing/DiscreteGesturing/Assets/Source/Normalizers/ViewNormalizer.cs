using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewNormalizer : Normalizer {

    public Transform userTransform;
    private Vector3 forward = new Vector3(0, 0, 1);

    public ViewNormalizer(Transform userTransform)
    {
        this.userTransform = userTransform;
    }

    public List<GTransform> Normalize(List<GTransform> data)
    {
        Vector3 centroid = new Vector3(0, 0, 0);
        int count = data.Count;
        List<GTransform> normalizedData = new List<GTransform>();

        for (int i = 0; i < data.Count; i++)
        {
            GTransform trans = data[i].Copy();
            centroid += trans.position;
            normalizedData.Add(trans);
        }


        centroid /= count;
        Vector3 userPosition = userTransform.position;
        Vector3 direction = (centroid - userPosition);
        Quaternion rotation = Quaternion.FromToRotation(direction, new Vector3(direction.x, 0, direction.z));

        centroid = rotation * (centroid - userPosition) + userPosition;
        direction = (centroid - userPosition);
        Quaternion rotation2 = Quaternion.FromToRotation(new Vector3(direction.x, 0, direction.z), forward);

        for (int i = 0; i < data.Count; i++)
        {
            GTransform trans = normalizedData[i];
            trans.position = rotation * (trans.position - userPosition) + userPosition;
            trans.position = rotation2 * (trans.position - userPosition) + userPosition;
        }



        return normalizedData;
    }

}
