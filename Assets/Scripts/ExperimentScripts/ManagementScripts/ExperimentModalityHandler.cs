using System.Collections.Generic;
using UnityEngine;

public enum ExperimentModalities
{
    DEACTIVATED, VISUAL, VISUAL_AUDIO, VISUAL_AUDIO_TACTILE
}

public class ExperimentModalityHandler
{
    public int Round {get; private set;} = -1;
    public List<ExperimentModalities> ModalitySequence {get; private set;}

    public ExperimentModalities CurrentModality {get; private set;} = ExperimentModalities.DEACTIVATED;

    public ExperimentModalityHandler(int experimentId)
    {
        LoadModalitySequence(experimentId);
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
        if(HasNextModality())
        {
            Round += 1;
            CurrentModality = ModalitySequence[Round];
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

    public void DeactivateModalitiesForStandby()
    {
        CurrentModality = ExperimentModalities.DEACTIVATED;
    }

    private void LoadModalitySequence(int experimentId)
    {
        ExperimentSequence[] sequences = ExperimentConfigLoader.FetchExperimentConfig().permutations;

        //checking if the experimentId is in bounds of the available sequences
        if(experimentId+1 > sequences.Length || experimentId < 0)
        {
            Debug.LogWarning("Tried to access experiment with id "+ experimentId +" which does not exist in the config file!");
            return;
        }

        ExperimentSequence sequence = sequences[experimentId];
        ModalitySequence = new List<ExperimentModalities>();

        for(int i = 0; i < sequence.modes.Length; i++)
        {
            ExperimentModalities nextModality = ConfigItemToModality(sequence.modes[i]);
            ModalitySequence.Add(nextModality);
        }
    }

    private ExperimentModalities ConfigItemToModality(int number)
    {
        return (ExperimentModalities)number;
    }
}
