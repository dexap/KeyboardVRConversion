using System.Collections;
using Manus.Hand;
using Manus.Haptics;
using Manus.Utility;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ManusGloveFingerHaptics : MonoBehaviour
{
    private float _duration = 0.2f;

    private float _intensity = 0.5f;
    
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
            _hand.data.SetFingerHaptic(_fingerHaptics.fingerType, _intensity);

            StartCoroutine(CancelImpulseCoroutine(_fingerHaptics.fingerType, _duration));
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
