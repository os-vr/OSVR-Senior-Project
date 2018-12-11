using UnityEngine;
using UnityEngine.EventSystems;

public class SetUITransformRay : MonoBehaviour
{
    [SerializeField]
    private OVRInputModule inputModule;
    [SerializeField]
    private OVRGazePointer gazePointer;

    public void SetUIRays()
    {
        inputModule.rayTransform = ControllerInfo.CONTROLLER_DATA_FOR_RAYS.transform;
        gazePointer.rayTransform = ControllerInfo.CONTROLLER_DATA_FOR_RAYS.transform;
    }
}