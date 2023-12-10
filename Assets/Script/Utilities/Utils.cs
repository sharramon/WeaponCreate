using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static float FindDistanceOnLocalXZPlane(Transform baseTransform, Transform compareTransform)
    {
        // Convert compareTransform position to baseTransform's local space
        Vector3 localPos = baseTransform.InverseTransformPoint(compareTransform.position);

        Vector3 localXZPos = new Vector3(localPos.x, 0, localPos.z);
        float distance = Vector3.Distance(new Vector3(0, 0, 0), localXZPos);

        return distance;
    }

    public static float FindDistanceOnLocalYZPlane(Transform baseTransform, Transform compareTransform)
    {
        // Convert compareTransform position to baseTransform's local space
        Vector3 localPos = baseTransform.InverseTransformPoint(compareTransform.position);

        Vector3 localXZPos = new Vector3(0, localPos.x, localPos.z);
        float distance = Vector3.Distance(new Vector3(0, 0, 0), localXZPos);

        return distance;
    }
}
