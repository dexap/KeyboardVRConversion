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
        _thumbHaptics.enabled = false;
        _indexHaptics.enabled = false;
        _middleHaptics.enabled = false;
        _ringHaptics.enabled = false;
        _pinkyHaptics.enabled = false;
    }

    public void EnableAllGloveKeyHaptics()
    {
        _thumbHaptics.enabled = true;
        _indexHaptics.enabled = true;
        _middleHaptics.enabled = true;
        _ringHaptics.enabled = true;
        _pinkyHaptics.enabled = true;
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
