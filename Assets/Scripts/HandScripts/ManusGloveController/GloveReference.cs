using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GloveReference : MonoBehaviour
{
    [Header("Glove Haptics")]
    [SerializeField]
    private ManusGloveFingerHaptics _thumbHaptics;
    [SerializeField]
    private ManusGloveFingerHaptics _indexHaptics;
    [SerializeField]
    private ManusGloveFingerHaptics _middleHaptics;
    [SerializeField]
    private ManusGloveFingerHaptics _ringHaptics;
    [SerializeField]
    private ManusGloveFingerHaptics _pinkyHaptics;
    
    [Header("Finger Vibration")]
    [SerializeField, Range(0, 1)]
    private float _vibrationIntensity = 0.2f;
    [SerializeField, Range(0, 1)]
    private float _vibrationDuration = 0.1f;

    [Header("Glove Poke Interactors")]
    [SerializeField]
    private XRPokeInteractor _thumbInteractor;
    [SerializeField]
    private XRPokeInteractor _indexInteractor;
    [SerializeField]
    private XRPokeInteractor _middleInteractor;
    [SerializeField]
    private XRPokeInteractor _ringInteractor;
    [SerializeField]
    private XRPokeInteractor _pinkyInteractor;


    public void DisableAllGloveKeyHaptics()
    {
        _thumbHaptics.HapticsActive = false;
        _indexHaptics.HapticsActive = false;
        _middleHaptics.HapticsActive = false;
        _ringHaptics.HapticsActive = false;
        _pinkyHaptics.HapticsActive = false;
    }

    public void EnableAllGloveKeyHaptics()
    {
        _thumbHaptics.HapticsActive = true;
        _indexHaptics.HapticsActive = true;
        _middleHaptics.HapticsActive = true;
        _ringHaptics.HapticsActive = true;
        _pinkyHaptics.HapticsActive = true;
        
        _thumbHaptics.Intensity = _vibrationIntensity;
        _indexHaptics.Intensity = _vibrationIntensity;
        _middleHaptics.Intensity = _vibrationIntensity;
        _ringHaptics.Intensity = _vibrationIntensity;
        _pinkyHaptics.Intensity = _vibrationIntensity;
        
        _thumbHaptics.Duration = _vibrationDuration;
        _indexHaptics.Duration = _vibrationDuration;
        _middleHaptics.Duration = _vibrationDuration;
        _ringHaptics.Duration = _vibrationDuration;
        _pinkyHaptics.Duration = _vibrationDuration;
    }

    public void DisableAllGloveKeyInteractors()
    {
        _thumbInteractor.enabled = false;
        _indexInteractor.enabled = false;
        _middleInteractor.enabled = false;
        _ringInteractor.enabled = false;
        _pinkyInteractor.enabled = false;
    }

    public void EnableAllGloveKeyInteractors()
    {
        _thumbInteractor.enabled = true;
        _indexInteractor.enabled = true;
        _middleInteractor.enabled = true;
        _ringInteractor.enabled = true;
        _pinkyInteractor.enabled = true;
    }
}
