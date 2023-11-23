using UnityEngine;
using IngameDebugConsole;

public class CommandScript : MonoBehaviour
{
    [SerializeField]
    private ExperimentController _experimentController;

    void Start()
    {
        DebugLogConsole.AddCommand<int>("xpstart", "Starts experiment with a given ID", XPStart);
        DebugLogConsole.AddCommand("modenow", "Displays the current modality of the experiment", ModeNow);
        DebugLogConsole.AddCommand("modenext", "Switches to the next modality of the experiment", ModeNext);
        DebugLogConsole.AddCommand("confloc", "Opens the config files location", OpenConfigFilePath);
    }

    private void XPStart(int experimentId)
    {
        _experimentController.StartExperiment(experimentId);
    }

    public void ModeNow()
    {
        DisplayCurrentModality();
    }

    private void ModeNext()
    {
        _experimentController.ActivateNextModality();
    }

    private void OpenConfigFilePath()
    {
        System.Diagnostics.Process.Start(ExperimentConfigLoader.ConfigFileLocation);
    }

    private void DisplayCurrentModality()
    {
        Debug.Log("Current mode: "+_experimentController.CurrentModality());
    }
}
