using UnityEngine;
using UnityEngine.InputSystem;

public class FinishNoVR : MonoBehaviour
{
    [SerializeField]
    FinishButton _finishButton;

    void Update()
    {
        // simulates the finish button through the right mouse button
        // since there is no finish button in NoVR 
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            _finishButton.OnButtonPress();
        }
    }

}
