using KeyInputVR.Keyboard;
using TMPro;
using UnityEngine;

public enum ExperimentStates
{
    INACTIVE, STANDBY, ACTIVE
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

    [SerializeField]
    private FinishButton _finishButton;

    private ExperimentModalityHandler _experimentModalityHandler;

    private bool HasExperimentHasStarted
    {
        get {return _currentExperimentState != ExperimentStates.INACTIVE ? true : false;}
    }

    private ExperimentStates _currentExperimentState = ExperimentStates.INACTIVE;

    private int _experimentId;

    //serializer reference gets renewed on every new round within one experiment
    private InputSerializer _inputSerializer;

    void Awake()
    {
        _keyPressManager.OnSendingSignal += LogInput;

        _keyPressManager.OnSendingSignalWithGloves += LogInputForGloves;

        _finishButton.OnFinishSignal += FinishRound;
    }

    void Start()
    {
        ApplyModalityChange(ExperimentModalities.DEACTIVATED);
    }

    private void LogInput(string input)
    {
        if(HasExperimentHasStarted)
        {
            _inputSerializer.LogInput(input);
        }
    }

    private void LogInputForGloves(string input, Manus.Utility.HandType handType, Manus.Utility.FingerType fingerType)
    {
        if(HasExperimentHasStarted)
        {
            _inputSerializer.LogInput(input, handType, fingerType);
        }
    }

    public void StartExperiment(int experimentId)
    {
        _experimentId = experimentId;

        if(HasExperimentHasStarted)
        {
            Debug.LogWarning("Another experiment has already been started. "+
            "To start another one, please finish the started experiment first or restart the program!");
            return;
        }

        Debug.Log("### STARTING EXPERIMENT ###");
        if(LoadExperiment(experimentId))
        {
            ActivateNextModality();
        }
        else
        {
            Debug.LogError("Startup aborted! Please try again with a valid ID");
        }
    }

    private bool LoadExperiment(int experimentId)
    {
        Debug.Log("Loading experiment from config file "+ ExperimentConfigLoader.ConfigFilePath);
        _experimentModalityHandler = new ExperimentModalityHandler(experimentId);
        
        if(_experimentModalityHandler.ModalitySequence == null)
        {
            Debug.LogWarning("Failed to load experiment with ID "+ experimentId +" !"+
            "\nDoes the provided ID exist in the config file?");

            return false;
        }
        else
        {
            Debug.Log("Loaded experiment with ID "+ experimentId);
            Debug.Log("Loaded experiment sequence:");
            foreach(ExperimentModalities modality in _experimentModalityHandler.ModalitySequence)
            {
                Debug.Log(modality);
            }

            return true;
        }
    }

    private void FinishRound()
    {
        if(!_experimentModalityHandler.HasNextModality())
        {
            EndExperiment();
        }
        else
        {
            Debug.Log("Round finished, experiment now in STANDBY! Ask the experimentee to answer the questionaires."+
            "To continue, enter the `"+CommandScript.CMD_NEXT_MODE+ "` command!");
            _currentExperimentState = ExperimentStates.STANDBY;
            _experimentModalityHandler.DeactivateModalitiesForStandby();
            ApplyModalityChange(_experimentModalityHandler.CurrentModality);
        }
    }

    public ExperimentModalities CurrentModality()
    {
        return _experimentModalityHandler.CurrentModality;
    }

    public void ResetCurrentModalityRound()
    {
        if(_currentExperimentState == ExperimentStates.ACTIVE)
        {
            Debug.Log("Resetting the current round for modality "+_experimentModalityHandler.CurrentModality);
            Debug.Log("Discarding old log file, moving it into "+InputSerializer.DiscardedFileLocation);
            _inputSerializer.DiscardLogFile();

            //clear the text of the input field
            _inputFieldInserting.text = "";

            _inputSerializer = new InputSerializer(_experimentId.ToString(), _experimentModalityHandler.Round);
            Debug.Log("Input data will be logged in "+ _inputSerializer.ResultFilePath);
        }
        else
        {
            Debug.LogWarning("You need to be in an active experiment state to reset the round of the modality!");
        }
    }

    public void ActivateNextModality()
    {
        //clear the text of the input field
        _inputFieldInserting.text = "";

        if(_experimentModalityHandler.SwitchToNextModality())
        {
            _currentExperimentState = ExperimentStates.ACTIVE;
            ApplyModalityChange(CurrentModality());
            Debug.Log("Entered modality: "+ CurrentModality());
            
            //creating new serializer for fresh experiment round
            _inputSerializer = new InputSerializer(_experimentId.ToString(), _experimentModalityHandler.Round);
            Debug.Log("Input data will be logged in "+ _inputSerializer.ResultFilePath);
        }
        else
        {
            EndExperiment();
        }
    }

    private void EndExperiment()
    {
        Debug.Log("Experiment over! You can start another experiment or close the program");
        _currentExperimentState = ExperimentStates.INACTIVE;
        _experimentModalityHandler.DeactivateModalitiesForStandby();
        ApplyModalityChange(_experimentModalityHandler.CurrentModality);
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