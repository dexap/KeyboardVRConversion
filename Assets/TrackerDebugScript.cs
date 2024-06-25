using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackerDebug : MonoBehaviour
{
    // Referenzen zu den Input Actions
    public InputActionProperty viveTrackerPositionAction;
    public InputActionProperty viveTrackerRotationAction;
    private Vector3 oldTrackerPosition;
    private Quaternion oldTrackerRotation;
    
    void Start()
    {
        // Input Actions aktivieren
        viveTrackerPositionAction.action.Enable();
        viveTrackerRotationAction.action.Enable();
        oldTrackerRotation = Quaternion.identity;
        oldTrackerPosition = Vector3.zero;
    }

    void Update()
    {
        if(oldTrackerPosition != viveTrackerPositionAction.action.ReadValue<Vector3>() || oldTrackerRotation != viveTrackerRotationAction.action.ReadValue<Quaternion>())
        {
            Debug.Log("New Tracker Position: " + viveTrackerPositionAction.action.ReadValue<Vector3>());
            Debug.Log("New Tracker Rotation: " + viveTrackerRotationAction.action.ReadValue<Quaternion>());
        }
        
    }

    private void OnDisable()
    {
        // Input Actions deaktivieren
        viveTrackerPositionAction.action.Disable();
        viveTrackerRotationAction.action.Disable();
    }
}