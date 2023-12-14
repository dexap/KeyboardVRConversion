using Hermes.Protocol;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera _vrCamera;

    [SerializeField]
    private Camera _noVRCamera;

    public void Start()
    {
        SwitchToVR();
    }


    public void SwitchToVR()
    {
        _vrCamera.enabled = true;
        _noVRCamera.enabled = false;
    }

    public void SwitchToNoVR()
    {
        _vrCamera.enabled = false;
        _noVRCamera.enabled = true;
    }
}
