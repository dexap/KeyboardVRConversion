using System.Collections;
using Manus.Hand;
using Manus.Haptics;
using Manus.Utility;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ManusGloveFingerHaptics : MonoBehaviour
{
    public float Duration { get; set; } = 0.1f;

    public float Intensity { get; set; } = 0.2f;
    
    [SerializeField]
    private FingerHaptics _fingerHaptics;

    private Hand _hand;

    public bool HapticsActive {get; set;} = true;
    
    void Awake()
    {
        _hand = _fingerHaptics.GetComponentInParent<Hand>();
    }

    public void SendImpulse(SelectEnterEventArgs eventArgs)
    {
        if(HapticsActive)
        {
            _hand.data.SetFingerHaptic(_fingerHaptics.fingerType, Intensity);

            StartCoroutine(CancelImpulseCoroutine(_fingerHaptics.fingerType, Duration));
        }
    }

    IEnumerator CancelImpulseCoroutine(FingerType fingerType, float delay)
    {   
        yield return new WaitForSeconds(delay);

        StopImpulse(fingerType);
    }

    private void StopImpulse(FingerType fingerType)
    {
        _hand.data.SetFingerHaptic(fingerType, 0);
    }
}
