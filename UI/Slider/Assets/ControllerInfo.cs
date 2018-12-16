using UnityEngine;
using static OVRInput;

public class ControllerInfo : MonoBehaviour
{
    [SerializeField]
    private Transform trackingSpace;
    public static Transform TRACKING_SPACE;
    public static Controller CONTROLLER;
    public static GameObject CONTROLLER_DATA_FOR_RAYS;

    private void Start()
    {
        TRACKING_SPACE = trackingSpace;
    }

    private void Update()
    {
        CONTROLLER = ((GetConnectedControllers() & (Controller.LTrackedRemote | Controller.RTrackedRemote) & Controller.LTrackedRemote) != Controller.None) ? Controller.LTrackedRemote : Controller.RTrackedRemote;
    }
}