using Hermes.Protocol;
using KeyInputVR.Keyboard;
using UnityEngine;

public enum ExperimentModalities
{
    DEACTIVATED, VISUAL, VISUAL_AUDIO, VISUAL_AUDIO_TACTILE
}

public class ExperimentController : MonoBehaviour
{
    [SerializeField]
    private KeyboardInfo _keyboardInfo; 

    [SerializeField]
    private GloveReference _leftGloveReference;
    [SerializeField]
    private GloveReference _rightGloveReference;

    private ExperimentStateHandler _experimentStateHandler;

    private bool _experimentHasStarted;


    // public void ChangeModality(ExperimentModalities modality)
    // {
    //     CurrentModality = modality;
    //     ApplyModalityChange(modality);
    // }

    void Start()
    {
        ApplyModalityChange(ExperimentModalities.DEACTIVATED);
    }

    public void StartExperiment(int experimentId)
    {
        if(_experimentHasStarted)
        {
            Debug.LogWarning("Another experiment has already been started. "+
            "To start another one, please finish the started experiment first or restart the program!");
            return;
        }

        Debug.Log("### STARTING EXPERIMENT ###");
        if(LoadExperiment(experimentId))
        {
            ActivateNextModality();
            _experimentHasStarted = true;
        }
        else
        {
            Debug.LogError("Startup aborted! Please try again with a valid ID");
        }
    }

    private bool LoadExperiment(int experimentId)
    {
        Debug.Log("Loading experiment from config file "+ ExperimentConfigLoader.ConfigFilePath);
        _experimentStateHandler = new ExperimentStateHandler(experimentId);
        if(_experimentStateHandler.ModalitySequence == null)
        {
            Debug.LogWarning("Failed to load experiment with ID "+ experimentId +" !"+
            "\nDoes the provided ID exist in the config file?");

            return false;
        }
        else
        {
            Debug.Log("Loaded experiment with ID "+ experimentId);
            Debug.Log("Loaded experiment sequence:");
            foreach(ExperimentModalities modality in _experimentStateHandler.ModalitySequence)
            {
                Debug.Log(modality);
            }

            return true;
        }
    }

    public ExperimentModalities CurrentModality()
    {
        return _experimentStateHandler.CurrentModality;
    }

    public void ActivateNextModality()
    {
        if(_experimentStateHandler.SwitchToNextModality())
        {
            ApplyModalityChange(CurrentModality());
            Debug.Log("Entered modality: "+ CurrentModality());
        }
        else
        {
            Debug.Log("Experiment over!");
            _experimentHasStarted = false;
        }
    }

    private void ApplyModalityChange(ExperimentModalities modality)
    {
        switch (modality)
        {
            case ExperimentModalities.DEACTIVATED:
                DeactivateInteractions();
                DeactivateAudio();
                DeactivateHaptics();
                break;
            case ExperimentModalities.VISUAL:
                ActivateInteractions();
                DeactivateAudio();
                DeactivateHaptics();
                break;
            case ExperimentModalities.VISUAL_AUDIO:
                ActivateInteractions();
                ActivateAudio();
                DeactivateHaptics();
                break;
            case ExperimentModalities.VISUAL_AUDIO_TACTILE:
                ActivateInteractions();
                ActivateAudio();
                ActivateHaptics();
                break;
            default:
                break;
        }
    }

    private void ActivateAudio()
    {
        _keyboardInfo.IsSoundFeedbackEnabled = true;
    }

    private void DeactivateAudio()
    {
        _keyboardInfo.IsSoundFeedbackEnabled = false;
    }

    private void ActivateHaptics()
    {
        _rightGloveReference.EnableAllGloveKeyHaptics();
        _leftGloveReference.EnableAllGloveKeyHaptics();
    }

    private void DeactivateHaptics()
    {
        _rightGloveReference.DisableAllGloveKeyHaptics();
        _leftGloveReference.DisableAllGloveKeyHaptics();
    }
    
    private void ActivateInteractions()
    {
        _rightGloveReference.EnableAllGloveKeyInteractors();
        _leftGloveReference.EnableAllGloveKeyInteractors();
    }

    private void DeactivateInteractions()
    {
        _rightGloveReference.DisableAllGloveKeyInteractors();
        _leftGloveReference.DisableAllGloveKeyInteractors();
    }
}

