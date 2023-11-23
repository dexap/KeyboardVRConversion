using KeyInputVR.Keyboard;
using TMPro;
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
    private KeyPressManager _keyPressManager; 

    [SerializeField]
    private GloveReference _leftGloveReference;
    [SerializeField]
    private GloveReference _rightGloveReference;

    [SerializeField]
    private TMP_InputField _inputFieldInserting;

    private ExperimentStateHandler _experimentStateHandler;

    private bool _experimentHasStarted;

    private int _experimentId;

    //serializer reference gets renewed on every new round within one experiment
    private InputSerializer _inputSerializer;

    void Awake()
    {
        _keyPressManager.OnSendingCharacter += LogInput;
    }

    void Start()
    {
        ApplyModalityChange(ExperimentModalities.DEACTIVATED);
    }

    private void LogInput(string input)
    {
        if(_experimentHasStarted)
        {
            _inputSerializer.LogInput(input);
        }
    }

    public void StartExperiment(int experimentId)
    {
        _experimentId = experimentId;

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
        //clear the text of the input field
        _inputFieldInserting.text = "";

        if(_experimentStateHandler.SwitchToNextModality())
        {
            ApplyModalityChange(CurrentModality());
            Debug.Log("Entered modality: "+ CurrentModality());
            _inputSerializer = new InputSerializer(_experimentId.ToString(), _experimentStateHandler.Round);
            Debug.Log("Input data will be logged in "+ _inputSerializer.ResultFilePath);
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

