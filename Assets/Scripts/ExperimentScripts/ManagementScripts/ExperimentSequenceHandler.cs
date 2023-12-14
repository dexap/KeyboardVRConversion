using System.Collections.Generic;
using UnityEngine;

public enum ExperimentModalities
{
    DEACTIVATED, NO_VR, VISUAL, VISUAL_AUDIO, VISUAL_AUDIO_TACTILE
}

public class ExperimentSequenceHandler
{
    public int Round {get; private set;} = -1;
    public List<ExperimentModalities> ModalitySequence {get; private set;}

    public List<string> TextSequence {get; private set;}

    public List<string> TextSequenceNames {get; private set;}

    public bool ErrorOnLoad {get; private set;} = false;

    public ExperimentModalities CurrentModality {get; private set;} = ExperimentModalities.DEACTIVATED;

    public string CurrentText {get; private set;} = null;

    public string CurrentTextName {get; private set;} = null;

    public ExperimentSequenceHandler(int experimentId)
    {
        LoadSequence(experimentId);

        if(ModalitySequence == null)
        {
            Debug.LogError("Unable to load the modality sequence from the config for experimentID `"+experimentId+"`! "+
            "Please make sure the config exists and is correctly configured! Make sure to check the spelling!");
            ErrorOnLoad = true;
        }

        if(TextSequence == null)
        {
            Debug.LogError("Unable to load the text sequence from the config for experimentID `"+experimentId+"`! "+
            "Please make sure the config exists and is correctly configured! Make sure to check the spelling!");
            ErrorOnLoad = true;
        }

        if(ModalitySequence.Count != TextSequence.Count)
        {
            Debug.LogError("Unequal amount of modalities and texts detected for experimentID `"+experimentId+"`! "+
            "This is very likely a config error! Please make sure that there is a text for each modality!"
            );
            ErrorOnLoad = true;
        }
    }

    /// <summary>
    /// Switches the state to the next modality element.
    /// </summary>
    /// <returns>
    /// True if there is a next element.
    /// False if there is no next element to switch to.
    /// A false return value indicates the end of the experiment cycle.
    /// </returns>
    public bool SwitchToNextModality()
    {
        if(HasNextModality() && HasNextText())
        {
            Round += 1;
            CurrentModality = ModalitySequence[Round];
            CurrentText = TextSequence[Round];
            CurrentTextName = TextSequenceNames[Round];
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns if there is a next modality to switch to
    /// </summary>
    /// <returns>
    /// True if there is a next element.
    /// False if there is no next element to switch to.
    /// A false return value indicates the end of the experiment cycle.
    /// </returns>
    public bool HasNextModality()
    {
        if(Round+1 < ModalitySequence.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns if there is a next text to switch to
    /// </summary>
    /// <returns>
    /// True if there is a next element.
    /// False if there is no next element to switch to.
    /// A false return value indicates the end of the experiment cycle.
    /// </returns>
    public bool HasNextText()
    {
        if(Round+1 < TextSequence.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeactivateModalitiesForStandby()
    {
        CurrentModality = ExperimentModalities.DEACTIVATED;
    }

    private void LoadSequence(int experimentId)
    {
        ExperimentSequence[] sequences = ExperimentConfigLoader.FetchExperimentConfig().permutations;

        if(!ExperimentIDExists(sequences, experimentId))
        {
            Debug.LogWarning("Tried to access experiment with id "+ experimentId +" which does not exist in the config file!");
            return;
        }

        ExperimentSequence sequence = sequences[experimentId];

        if(sequence != null)
        {
            LoadModalitySequence(sequence);
            LoadTextSequence(sequence);
        }
    }

    private void LoadModalitySequence(ExperimentSequence sequence)
    {
        //stop loading if modes does not exist
        if(sequence.modes == null)
            return;

        ModalitySequence = new List<ExperimentModalities>();

        for(int i = 0; i < sequence.modes.Length; i++)
        {
            ExperimentModalities nextModality = ConfigItemToModality(sequence.modes[i]);
            ModalitySequence.Add(nextModality);
        }
    }

    private void LoadTextSequence(ExperimentSequence sequence)
    {
        //stop loading if text does not exist
        if(sequence.texts == null)
            return;

        TextSequence = new List<string>();
        TextSequenceNames = new List<string>();

        for(int i = 0; i < sequence.texts.Length; i++)
        {
            string text = ExperimentTextLoader.FetchText(sequence.texts[i]);
            if(text != null)
            {
                TextSequence.Add(text);
                TextSequenceNames.Add(sequence.texts[i]);
            }
        }
    }

    private bool ExperimentIDExists(ExperimentSequence[] sequences, int experimentId)
    {
        //checking if the experimentId is in bounds of the available sequences
        if(experimentId+1 > sequences.Length || experimentId < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private ExperimentModalities ConfigItemToModality(int number)
    {
        return (ExperimentModalities)number;
    }
}
