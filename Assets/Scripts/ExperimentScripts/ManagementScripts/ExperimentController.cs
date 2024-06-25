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
    private PhysicalInputManager _physicalInputManager;

    [SerializeField]
    private GloveReference _leftGloveReference;
    [SerializeField]
    private GloveReference _rightGloveReference;

    [SerializeField, Tooltip("Holds the written text during the experiment")]
    private TMP_InputField _inputFieldInserting;

    [SerializeField, Tooltip("Holds the text that has to be copied during the experiment")]
    private TMP_InputField _secondaryTextField;

    private string _secondaryFieldDefaultText;

    private readonly string _secondaryFieldInStandby =
    "Runde abgeschlossen! \nBitte erkundige dich nach den nächsten Anweisungen um fortzufahren.";

    [SerializeField]
    private FinishButton _finishButton;

    [SerializeField]
    private FinishButton _finishButtonNoVR;

    [SerializeField]
    private CameraManager _cameraManager;

    private ExperimentSequenceHandler _experimentSequenceHandler;

    private bool HasExperimentStarted => _currentExperimentState != ExperimentStates.INACTIVE;

    private ExperimentStates _currentExperimentState = ExperimentStates.INACTIVE;

    private int _experimentId;

    //serializer reference gets renewed on every new round within one experiment
    private InputSerializer _inputSerializer;

    void Awake()
    {
        _keyPressManager.OnSendingSignal += LogInput;

        _keyPressManager.OnSendingSignalWithGloves += LogInputForGloves;

        _physicalInputManager.OnSendingSignal += LogInputForPhysicalKeyboard;

        _finishButton.OnFinishSignal += FinishRound;

        _finishButtonNoVR.OnFinishSignal += FinishRound;
    }

    void Start()
    {
        ApplyModalityChange(ExperimentModalities.DEACTIVATED);
        _secondaryFieldDefaultText = _secondaryTextField.text;
    }

    private void LogInput(string input)
    {
        if(HasExperimentStarted)
        {
            _inputSerializer.LogInput(input);
        }
    }

    private void LogInputForGloves(string input, Manus.Utility.HandType handType, Manus.Utility.FingerType fingerType)
    {
        if(HasExperimentStarted)
        {
            _inputSerializer.LogInput(input, handType, fingerType);
        }
    }

    private void LogInputForPhysicalKeyboard(string input)
    {
        if(!HasExperimentStarted)
            return;

        if(_experimentSequenceHandler.CurrentModality == ExperimentModalities.NO_VR)
        {
            _inputSerializer.LogInput(input);
        }
    }

    public void StartExperiment(int experimentId)
    {
        _experimentId = experimentId;

        if(HasExperimentStarted)
        {
            Debug.LogWarning("Another experiment has already been started and is still running. "+
            "To start another one, please finish the started experiment first or restart the program!");
            return;
        }

        Debug.Log("### STARTING EXPERIMENT ###");
        if(LoadExperiment(experimentId))
        {
            ActivateNextRoundOfSequence();
        }
        else
        {
            Debug.LogError("Startup aborted!");
        }
    }

    private bool LoadExperiment(int experimentId)
    {
        Debug.Log("Loading experiment from config file "+ ExperimentConfigLoader.ConfigFilePath);
        _experimentSequenceHandler = new ExperimentSequenceHandler(experimentId);
        
        if(_experimentSequenceHandler.ModalitySequence == null)
        {
            Debug.LogWarning("Failed to load experiment with ID `"+ experimentId +"`! "+
            "\nDoes the provided ID exist in the config file?");
            return false;
        }
        
        //final check if there have been errors while loading the config
        if(_experimentSequenceHandler.ErrorOnLoad)
        {
            Debug.LogError("Failed to load experiment with ID `"+ experimentId +"`! ");
            return false;
        }
        else
        {
            Debug.Log("Loaded experiment with ID "+ experimentId);
            Debug.Log("Loaded experiment sequence:");
            foreach(ExperimentModalities modality in _experimentSequenceHandler.ModalitySequence)
            {
                Debug.Log(modality);
            }

            return true;
        }
    }

    private void FinishRound()
    {
        if(_currentExperimentState == ExperimentStates.INACTIVE)
        {
            return;
        }

        if(!_experimentSequenceHandler.HasNextModality())
        {   
            EndExperiment();
        }
        else
        {
            Debug.Log("Round finished, experiment now in STANDBY! Please ask the experimentee to answer the questionaires now."+
            "To continue, enter the `"+CommandScript.CMD_NEXT_MODE+ "` command!");
            _currentExperimentState = ExperimentStates.STANDBY;
            _experimentSequenceHandler.DeactivateModalitiesForStandby();
            ApplyModalityChange(CurrentModality());
            _secondaryTextField.text = _secondaryFieldInStandby;
        }
    }

    public ExperimentModalities CurrentModality()
    {
        return _experimentSequenceHandler.CurrentModality;
    }

    public void ResetCurrentModalityRound()
    {
        if(_currentExperimentState == ExperimentStates.ACTIVE)
        {
            Debug.Log("Resetting the current round for modality "+_experimentSequenceHandler.CurrentModality);
            Debug.Log("Discarding old log file, moving it into "+InputSerializer.DiscardedFileLocation);
            _inputSerializer.DiscardLogFile();
            _inputFieldInserting.text = "";
            
            _inputSerializer = new InputSerializer(
                _experimentId.ToString(), 
                _experimentSequenceHandler.Round,
                CurrentModality(),
                _experimentSequenceHandler.CurrentText);
             
        }
        else
        {
            Debug.LogWarning("You need to be in an active experiment state to reset the round of the modality!");
        }
    }

    public void ActivateNextRoundOfSequence()
    {
        //clear the text of the input field
        _inputFieldInserting.text = "";

        if(_experimentSequenceHandler.SwitchToNextModality())
        {
            _currentExperimentState = ExperimentStates.ACTIVE;
            ApplyModalityChange(CurrentModality());
            _secondaryTextField.text = _experimentSequenceHandler.CurrentText;
            
            Debug.Log("Entered modality: "+ CurrentModality());
            Debug.Log("Using text: "+ _experimentSequenceHandler.CurrentText);
            
            _inputSerializer = new InputSerializer(
                _experimentId.ToString(), 
                _experimentSequenceHandler.Round,
                CurrentModality(),
                _experimentSequenceHandler.CurrentText);
            
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
        _experimentSequenceHandler.DeactivateModalitiesForStandby();
        ApplyModalityChange(_experimentSequenceHandler.CurrentModality);
        _secondaryTextField.text = _secondaryFieldDefaultText;
    }

    private void ApplyModalityChange(ExperimentModalities modality)
    {
        switch (modality)
        {
            case ExperimentModalities.DEACTIVATED:
                DeactivateInteractions();
                DeactivateAudio();
                DeactivateHaptics();
                DisableNoVREnvironment();
                break;
            case ExperimentModalities.NO_VR:
                DeactivateInteractions();
                DeactivateAudio();
                DeactivateHaptics();
                EnableNoVREnvironment();
                break;
            case ExperimentModalities.VISUAL:
                ActivateInteractions();
                DeactivateAudio();
                DeactivateHaptics();
                DisableNoVREnvironment();
                break;
            case ExperimentModalities.VISUAL_AUDIO:
                ActivateInteractions();
                ActivateAudio();
                DeactivateHaptics();
                DisableNoVREnvironment();
                break;
            case ExperimentModalities.VISUAL_AUDIO_TACTILE:
                ActivateInteractions();
                ActivateAudio();
                ActivateHaptics();
                DisableNoVREnvironment();
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

        // _rightGloveReference.SetGloveKeyHapticsIntensity(0.3f);
        // _leftGloveReference.SetGloveKeyHapticsIntensity(0.3f);
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

    private void EnableNoVREnvironment()
    {
        _physicalInputManager.InputEnabled = true;
        
        _finishButton.InteractionsEnabled = false;
        _finishButtonNoVR.InteractionsEnabled = true;

        _cameraManager.SwitchToNoVR();
    }

    private void DisableNoVREnvironment()
    {
        _physicalInputManager.InputEnabled = false;
        
        _finishButton.InteractionsEnabled = true;
        _finishButtonNoVR.InteractionsEnabled = false;

        _cameraManager.SwitchToVR();
    }
}