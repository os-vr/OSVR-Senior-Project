using UnityEngine;
using UnityEngine.EventSystems;
using static OVRInput;

[RequireComponent(typeof(LineRenderer))]
public class ControllerPointer : MonoBehaviour
{
    [SerializeField]
    private SetUITransformRay uiRays;
    private LineRenderer pointerLine;
    private GameObject tempPointerVals;

    private void Start()
    {
        tempPointerVals = new GameObject();
        tempPointerVals.transform.parent = transform;
        tempPointerVals.name = "tempPointerVals";
        pointerLine = gameObject.GetComponent<LineRenderer>();
        pointerLine.useWorldSpace = true;

        ControllerInfo.CONTROLLER_DATA_FOR_RAYS = tempPointerVals;
        uiRays.SetUIRays();
    }

    private void LateUpdate()
    {
        Quaternion rotation = GetLocalControllerRotation(ControllerInfo.CONTROLLER);
        Vector3 position = GetLocalControllerPosition(ControllerInfo.CONTROLLER);
        Vector3 pointerOrigin = ControllerInfo.TRACKING_SPACE.position + position;
        Vector3 pointerProjectedOrientation = ControllerInfo.TRACKING_SPACE.position + (rotation * Vector3.forward);
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        Vector3 pointerDrawStart = pointerOrigin - pointerProjectedOrientation * 0.05f;
        Vector3 pointerDrawEnd = pointerOrigin + pointerProjectedOrientation * 500.0f;
        pointerLine.SetPosition(0, pointerDrawStart);
        pointerLine.SetPosition(1, pointerDrawEnd);

        tempPointerVals.transform.position = pointerDrawStart;
        tempPointerVals.transform.rotation = rotation;
    }
}