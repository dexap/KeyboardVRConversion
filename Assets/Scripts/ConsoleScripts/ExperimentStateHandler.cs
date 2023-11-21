using System.Collections.Generic;
using UnityEngine;

public class ExperimentStateHandler
{
    private int _index = -1;
    public List<ExperimentModalities> ModalitySequence {get; private set;}

    public ExperimentModalities CurrentModality {get; private set;} = ExperimentModalities.DEACTIVATED;

    public ExperimentStateHandler(int experimentId)
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
        if(_index+1 < ModalitySequence.Count)
        {
            _index += 1;
            CurrentModality = ModalitySequence[_index];
            return true;
        }
        else
        {
            return false;
        }
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
